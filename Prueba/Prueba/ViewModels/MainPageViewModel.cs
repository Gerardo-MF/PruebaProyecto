﻿using Prueba.Models;
using System;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using Prueba.Views;
using System.Runtime.CompilerServices;

namespace Prueba.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Maestro m;
        Chat viewChat;
        AvisosGenerales generales;
        private Boolean contactos;
        private Boolean chatsContact;
        private Boolean Avisos;
        private Color btnChatCont;
        private Color btnContac;
        private Color btnAvisos;

        public MainPageViewModel(Maestro maestro)
        {
            BCBtnChats = Color.FromHex("ffffff");
            BCBtnAvisos = Color.FromHex("ffffff");
            BCBtnContactos = Color.FromHex("ffffff");
            ChatContactVisible = true;
            ContactosVisible = false;
            Avisos = false;
            Descargar(maestro);       
            VerChatCommand = new Command<Alumno>(VerChat);
            SeleccionarBoton = new Command<String>(Seleccionar);
            ListaAlumnos = App.MainAvisos.GetGrupoAlumnos();
            ListaAvisos = App.MainAvisos.GetAvisosGenerales();
            
        }

        public Command<Alumno> VerChatCommand { get; set; }
        public Command<String> SeleccionarBoton { get; set; }

        public Color BCBtnContactos
        {
            get { return btnContac; }
            set { btnContac = value; Actualizar(); }
        }

        public Color BCBtnAvisos
        {
            get { return btnAvisos; }
            set { btnAvisos = value; Actualizar(); }
        }

        public Color BCBtnChats
        {
            get { return btnChatCont; }
            set { btnChatCont = value; Actualizar(); }
        }

        public Boolean ChatContactVisible
        {
            get { return chatsContact; }
            set { chatsContact = value; Actualizar(); }
        }

        public Boolean ContactosVisible
        {
            get { return contactos; }
            set { contactos = value; Actualizar(); }
        }

        public Boolean AvisosVisible
        {
            get { return Avisos; }
            set { Avisos = value; Actualizar(); }
        }


        public List<Alumno> ListaAlumnos { get; set; }
        public List<AvisosGenerales> ListaAvisos { get; set; }
       

        public async void Descargar(Maestro maestro)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await App.MainAvisos.DescargarGrupo(maestro.IdMaestro, maestro.IdEscuela);
            }
            else
            {
                IMessage mensaje = DependencyService.Get<IMessage>();
                mensaje.ShowToast("Sin conexión a internet");
            }
        }

        private void VerChat(Alumno alumno)
        {
            if (Connectivity.NetworkAccess==NetworkAccess.Internet)
            {
                if (viewChat==null)
                {
                    viewChat = new Chat();
                }

                ChatViewModel chatViewModel = new ChatViewModel(alumno.Clave);
                viewChat.BindingContext = chatViewModel;
                App.Current.MainPage.Navigation.PushAsync(viewChat);
            }
            else
            {
                IMessage mensaje = DependencyService.Get<IMessage>();
                mensaje.ShowToast("Sin conexión a internet");
            }
        }

        public async void DescargarAvisos(Escuela t)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await App.MainAvisos.DescargarAvisosGenerales(t.NombreEscuela);
            }

            else
            {
                IMessage mensaje = DependencyService.Get<IMessage>();
                mensaje.ShowToast("Sin conexión a internet");
            }

        }


       



        private void Seleccionar(String tipo)
        {
            if (tipo == "Chat")
            {
                BCBtnChats = Color.FromHex("EF8012");
                BCBtnContactos = Color.FromHex("ffffff");
                BCBtnAvisos = Color.FromHex("ffffff");
                AvisosVisible = false;
                ContactosVisible = false;
                ChatContactVisible = true;
            }
            else if (tipo == "Contactos")
            {
                BCBtnChats = Color.FromHex("ffffff");
                BCBtnAvisos = Color.FromHex("ffffff");
                BCBtnContactos = Color.FromHex("EF8012");
                AvisosVisible = false;
                ContactosVisible = true;
                ChatContactVisible = false;
            }
            else
            {
                BCBtnChats = Color.FromHex("ffffff");
                BCBtnContactos = Color.FromHex("ffffff");
                BCBtnAvisos = Color.FromHex("EF8012");
                AvisosVisible = true;
                ContactosVisible = false;
                chatsContact = false;
            }
           

        }

        void Actualizar([CallerMemberName] String nom="") 
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nom));
        }


    }
}
