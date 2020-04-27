﻿using System;
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
        }

        public Maestro GetMaestro()
        {
            return sQLiteConnection.Table<Maestro>().FirstOrDefault();
        }

        public List<Alumno> GetGrupoAlumnos()
        {
            return new List<Alumno>(sQLiteConnection.Table<Alumno>());
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

        }

        public async Task DescargarGrupo(Int32 idMaestro, Int32 idEscuela)
        {
            if (sQLiteConnection.Table<Grupo>().Count() == 0)
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
                        sQLiteConnection.InsertAll(g.ListaAlumnos);
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

        public async Task DescargarChat()
        {

        }



    }
}
