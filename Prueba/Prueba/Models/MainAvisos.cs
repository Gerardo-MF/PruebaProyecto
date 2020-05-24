using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<AvisosGenerales> GetAvisosGenerales(Int32 idEscuela)
        {
            return new List<AvisosGenerales>(sQLiteConnection.Table<AvisosGenerales>().Where(x => x.IdEscuela == idEscuela));
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
            return new List<Avisos>(sQLiteConnection.Table<Avisos>().Where(x => x.ClaveAlumno == Clave).OrderBy(x=>x.fechaEnviado));
        }

        public async Task<Boolean> IniciarSesion(String clave, String password, Int32 idEscuela)
        {
            if (sQLiteConnection.Table<Maestro>().Count(x => x.Clave == clave&&x.IdEscuela==idEscuela) == 0)
            {
                
                    httpClient = new HttpClient();
                    Dictionary<String, String> datos = new Dictionary<String, String>()
                {
                    {"clave",clave},
                    {"password",password},
                    {"idEscuela",idEscuela.ToString()}
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
                var maestro = sQLiteConnection.Table<Maestro>().FirstOrDefault(x => x.Clave == clave&&x.IdEscuela==idEscuela);
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

        public async Task DescargarAvisosGenerales(string NombreEscuela,Int32 idEscuela)
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
                        foreach (AvisosGenerales item in avisos)
                        {
                            item.IdEscuela = idEscuela;
                            sQLiteConnection.InsertOrReplace(item);
                        }
                    }
                }
            }
        }



        public async Task EnviarAvisos(Int32 idmaestro, Int32 idalumno, String contenido, DateTime fechaEnviar, DateTime fechaCaducidad, String clavealumno)
        {
            HttpClient httpClient = new HttpClient();
            Dictionary<String, String> datos = new Dictionary<String, String>()
            {
                {"idMaestro",idmaestro.ToString() },
                {"idAlumno",idalumno.ToString() },
                {"contenido",contenido},
                {"FechaEnviar", $"{fechaEnviar.Year}-{fechaEnviar.Month}-{fechaEnviar.Day}"},
                {"FechaCaducidad",$"{fechaCaducidad.Year}-{fechaCaducidad.Month}-{fechaCaducidad.Day}" }
            };
            var respuesta = await httpClient.PostAsync("https://avisosprimaria.itesrc.net/api/MaestrosApp/EnviarAviso", new FormUrlEncodedContent(datos));
            if (respuesta.IsSuccessStatusCode)
            {
                String datosRespuesta = await respuesta.Content.ReadAsStringAsync();
                var dat = JsonConvert.DeserializeObject<Int32>(datosRespuesta);
                Avisos a = new Avisos()
                {
                    IdAvisosEnviados = dat,
                    Contenido = contenido,
                    IdMaestro = idmaestro,
                    IdAlumno = idalumno,
                    fechaEnviado = fechaEnviar,
                    ClaveAlumno = clavealumno
                };
                sQLiteConnection.Insert(a);
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
                throw new Exception("No se han podido descargar los mensajes.");
            }

        }


    }
}
