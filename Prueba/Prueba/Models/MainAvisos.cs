using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Essentials;

namespace Prueba.Models
{
    /* https://avisosprimaria.itesrc.net/api/MaestrosApp/login //clave(String),password(String),idEscuela(int?)
       https://avisosprimaria.itesrc.net/api/MaestrosApp/GetGrupo //idMaestro(int?), idEscuela(integer?)
         */
    public class MainAvisos
    {

        SQLiteConnection sQLiteConnection;
        HttpClient httpClient;

        public MainAvisos()
        {
            sQLiteConnection = new SQLiteConnection($"{App.PersonalFolder}/Avisos");
            sQLiteConnection.CreateTable<Maestro>();
            //sQLiteConnection.CreateTable<Grupo>();
            sQLiteConnection.CreateTable<Alumno>();
            sQLiteConnection.CreateTable<Escuela>();
            sQLiteConnection.CreateTable<AvisosGenerales>();
            sQLiteConnection.CreateTable<Avisos>();
        }

        public List<AvisosGenerales> GetAvisosGenerales()
        {
            return new List<AvisosGenerales>(sQLiteConnection.Table<AvisosGenerales>());
        }

        public Maestro GetMaestro()
        {
            return sQLiteConnection.Table<Maestro>().FirstOrDefault();
        }

        public List<Alumno> GetGrupoAlumnos()
        {
            return new List<Alumno>(sQLiteConnection.Table<Alumno>());
        }

        public List<Escuela> GetEscuelas()
        {
            return new List<Escuela>(sQLiteConnection.Table<Escuela>());
        }
        public List<Avisos> GetBAvisosByAlumno(String Clave)
        {
            return new List<Avisos>(sQLiteConnection.Table<Avisos>().Where(x => x.ClaveAlumno == Clave));
        }

        public async Task<Boolean> IniciarSesion(String clave, String password, String idEscuela)
        {
            if (sQLiteConnection.Table<Maestro>().Count() == 0)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    httpClient = new HttpClient();
                    Dictionary<String, String> datos = new Dictionary<String, String>()
                {
                    {"clave",clave},
                    {"password",password},
                    {"idEscuela",idEscuela}
                };

                    HttpResponseMessage respuesta = await httpClient.PostAsync("https://avisosprimaria.itesrc.net/api/MaestrosApp/login", new FormUrlEncodedContent(datos));

                    if (respuesta.IsSuccessStatusCode)
                    {
                        String datosRespuesta = await respuesta.Content.ReadAsStringAsync();
                        Maestro m = JsonConvert.DeserializeObject<Maestro>(datosRespuesta);
                        sQLiteConnection.Insert(m);
                        return true;
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                else
                {
                    throw new ArgumentException("Sin conexión a Internet");
                }
            }
            else
            {
                return true;
            }

        }

        public async Task DescargarEscuelas()
        {
            if (sQLiteConnection.Table<Escuela>().Count() == 0)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    httpClient = new HttpClient();
                    HttpResponseMessage respuesta = await httpClient.GetAsync("https://avisosprimaria.itesrc.net/api/AdminApp/GetEscuelas");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        String datosRespuesta = await respuesta.Content.ReadAsStringAsync();
                        List<Escuela> escuelas = JsonConvert.DeserializeObject<List<Escuela>>(datosRespuesta);
                        sQLiteConnection.InsertAll(escuelas);
                    }
                }
            }
        }

        public async Task DescargarGrupo(Int32 idMaestro, Int32 idEscuela)
        {
            if (sQLiteConnection.Table<Alumno>().Count() == 0)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    httpClient = new HttpClient();
                    Dictionary<String, String> datos = new Dictionary<String, String>()
                {
                    {"idMaestro",idMaestro.ToString()},
                    {"idEscuela",idEscuela.ToString() }
                };

                    HttpResponseMessage respuesta = await httpClient.PostAsync("https://avisosprimaria.itesrc.net/api/MaestrosApp/GetGrupo", new FormUrlEncodedContent(datos));
                    if (respuesta.IsSuccessStatusCode)
                    {
                        String datosRespuesta = await respuesta.Content.ReadAsStringAsync();
                        Grupo g = JsonConvert.DeserializeObject<Grupo>(datosRespuesta);
                        sQLiteConnection.InsertAll(g.Alumnos);
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                else
                {
                    throw new ArgumentException("Sin conexión a Internet");
                }
            }

        }



        public async Task DescargarAvisosGenerales(string NombreEscuela)
        {
            if (sQLiteConnection.Table<AvisosGenerales>().Count() == 0)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage datos = await client.GetAsync($"https://avisosPrimaria.itersc.net/api/AvisosGenerales/NombreEscuela/{NombreEscuela}");
                    if (datos.IsSuccessStatusCode)
                    {
                        string datosRespuesta = await datos.Content.ReadAsStringAsync();
                        List<AvisosGenerales> avisos = JsonConvert.DeserializeObject<List<AvisosGenerales>>(datosRespuesta);
                        sQLiteConnection.InsertAll(avisos);
                    }
                }
            }
        }



        public async Task DescargarAvisos(String Clave)
        {
            if (sQLiteConnection.Table<Avisos>().Count(x=>x.ClaveAlumno==Clave) == 0)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    httpClient = new HttpClient();
                    Dictionary<String, String> datos = new Dictionary<String, String>()
                {
                    {"clave",Clave}
                };

                    HttpResponseMessage respuesta = await httpClient.PostAsync("https://avisosprimaria.itesrc.net/api/AlumnosApp/AvisosByClaveAlumno", new FormUrlEncodedContent(datos));
                    if (respuesta.IsSuccessStatusCode)
                    {
                        String datosRespuesta = await respuesta.Content.ReadAsStringAsync();
                        List<Avisos> lista = JsonConvert.DeserializeObject<List<Avisos>>(datosRespuesta);
                        lista.ForEach(x => { x.ClaveAlumno = Clave; });
                        sQLiteConnection.InsertAll(lista);
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                else
                {
                    throw new ArgumentException("Sin conexión a Internet");
                }
            }
        }




    }
}
