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
            return new List<AvisosGenerales>(sQLiteConnection.Table<AvisosGenerales>().OrderBy(x=>x.FechaEnviado));
        }


        public Maestro GetMaestro(String Clave)
        {
            return sQLiteConnection.Table<Maestro>().FirstOrDefault(x => x.Clave == Clave);
        }

        public List<Alumno> GetGrupoAlumnosByIdMaestro(Int32 idmaestro)
        {
            return new List<Alumno>(sQLiteConnection.Table<Alumno>().Where(x => x.IdMaestro == idmaestro));
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
            if (sQLiteConnection.Table<Maestro>().Count(x => x.Clave == clave) == 0)
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
                        m.Clave = clave;
                        m.Password = password;
                        sQLiteConnection.Insert(m);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    throw new ArgumentException("Sin conexión a Internet");
                }
            }
            else
            {
                var maestro = sQLiteConnection.Table<Maestro>().FirstOrDefault(x => x.Clave == clave);
                if (maestro != null)
                {
                    if (maestro.Password != password)
                    {
                        throw new ArgumentException("El password es incorrecto");
                    }
                    return true;
                }
                else
                {
                    throw new ArgumentException("El usuario no existe");
                }
            }
        }

        public async Task DescargarEscuelas()
        {
            if (sQLiteConnection.Table<Escuela>().Count() == 0)
            {
                    httpClient = new HttpClient();
                    HttpResponseMessage respuesta = await httpClient.GetAsync("https://avisosprimaria.itesrc.net/api/AdminApp/GetEscuelas");
                    if (respuesta.IsSuccessStatusCode)
                    {
                       String datosRespuesta = await respuesta.Content.ReadAsStringAsync();
                       List<Escuela> escuelas = JsonConvert.DeserializeObject<List<Escuela>>(datosRespuesta);
                       foreach (Escuela escuela in escuelas)
                       {
                        sQLiteConnection.InsertOrReplace(escuela);
                       }
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
                    HttpResponseMessage datos = await client.GetAsync($"https://avisosprimaria.itesrc.net/api/AvisosGenerales/NombreEscuela/{NombreEscuela}");
                    if (datos.IsSuccessStatusCode)
                    {
                        string datosRespuesta = await datos.Content.ReadAsStringAsync();
                        List<AvisosGenerales> avisos = JsonConvert.DeserializeObject<List<AvisosGenerales>>(datosRespuesta);
                        sQLiteConnection.InsertAll(avisos);
                    }
                }
            }
        }



        public async Task EnviarAvisos(Int32 idmaestro, Int32 idalumno, String contenido, DateTime fechaEnviar, DateTime fechaCaducidad, String clavealumno)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(50);
            Dictionary<String, String> datos = new Dictionary<String, String>()
            {
                {"idMaestro",idmaestro.ToString() },
                {"idAlumno",idalumno.ToString() },
                {"contenido",contenido},
                {"FechaEnviar", fechaEnviar.ToString()},
                {"FechaCaducidad",fechaCaducidad.ToString() }
            };
            var respuesta = await httpClient.PostAsync("https://avisosprimaria.itesrc.net/api/MaestrosApp/EnviarAviso", new FormUrlEncodedContent(datos));
            if (respuesta.IsSuccessStatusCode)
            {
                String datosRespuesta = await respuesta.Content.ReadAsStringAsync();
            }
            else
            {
                throw new ArgumentException("No se ha podido enviar el aviso");
            }

        }

        public async Task DescargarGrupo(Int32 idMaestro, Int32 idEscuela)
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
                foreach (Alumno alumno in g.Alumnos)
                {
                    alumno.IdMaestro = idMaestro;
                    sQLiteConnection.InsertOrReplace(alumno);
                }

                }
                else
                {
                    throw new ArgumentException("Ha ocurrido un error al enviar la solicitud");
                }

        }

        public async Task DescargarAvisos(String Clave)
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
               foreach (var avisos in lista)
               {
                 avisos.ClaveAlumno = Clave;
                 sQLiteConnection.InsertOrReplace(avisos);
               }
            }
            else
            {
                throw new Exception();
            }
            
        }


    }
}
