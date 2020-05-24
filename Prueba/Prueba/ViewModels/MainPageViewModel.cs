using Prueba.Models;
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
        IMessage mensaje = DependencyService.Get<IMessage>();
        readonly Maestro m;
        readonly Escuela e;
        AllChats viewAllChats;
        List<Alumno> listaAlumnos;
        List<AvisosGenerales> listaAvisosGenerales;
        private Boolean contactos;
        private Boolean chatsContact;
        private Boolean Avisos;
        private Color btnChatCont;
        private Color btnContac;
        private Color btnAvisos;

        public MainPageViewModel(Maestro maestro,Escuela escuela)
        {
            e = escuela;
            m = maestro;
            Descargar(maestro);
            DescargarAvisos();
            BCBtnChats = Color.FromHex("EF8012");
            BCBtnAvisos = Color.FromHex("ffffff");
            BCBtnContactos = Color.FromHex("ffffff");
            ChatContactVisible = true;
            ContactosVisible = false;
            Avisos = false;
            VerChatCommand = new Command<Alumno>(VerChat);
            SeleccionarBoton = new Command<String>(Seleccionar);           
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command<Alumno> VerChatCommand { get; set; }
        public Command<String> SeleccionarBoton { get; set; }
        public List<Alumno> ListaAlumnos 
        {
            get { return listaAlumnos; }
            set { listaAlumnos = value; Actualizar(); }
        }
        public List<AvisosGenerales> ListaAvisosGenerales 
        {
            get { return listaAvisosGenerales; }
            set { listaAvisosGenerales = value; Actualizar(); }
        }

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
       

        public async void Descargar(Maestro maestro)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await App.MainAvisos.DescargarGrupo(maestro.IdMaestro, maestro.IdEscuela);
            }
            else
            {
                mensaje.ShowToast("Sin conexión a internet");
            }
            ListaAlumnos = App.MainAvisos.GetGrupoAlumnosByIdMaestro(maestro.IdMaestro);
        }

        private async void DescargarAvisos()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await App.MainAvisos.DescargarAvisosGenerales(e.NombreEscuela,e.IdEscuela);
            }
            ListaAvisosGenerales = App.MainAvisos.GetAvisosGenerales(e.IdEscuela);
        }

        private void VerChat(Alumno alumno)
        {
            if (viewAllChats==null)
            {
                viewAllChats = new AllChats();
            }

            ChatViewModel chatViewModel = new ChatViewModel(alumno.Clave,alumno.IdAlumno,m.IdMaestro);
            chatViewModel.NombreAlumno = alumno.Nombre;
            viewAllChats.BindingContext = chatViewModel;
            App.Current.MainPage.Navigation.PushAsync(viewAllChats);
            
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
                ContactosVisible = false;
                ChatContactVisible = false;
                AvisosVisible = true;
            }
           

        }

        void Actualizar([CallerMemberName] String nom="") 
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nom));
        }


    }
}
