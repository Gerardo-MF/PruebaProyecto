using Prueba.Models;
using System;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using Prueba.Views;

namespace Prueba.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Maestro m;
        Chat viewChat;

        public MainPageViewModel(Maestro maestro)
        {
            Descargar(maestro);
            VerChatCommand = new Command<Alumno>(VerChat);
            ListaAlumnos = App.MainAvisos.GetGrupoAlumnos();
        }
        
        public Command<Alumno> VerChatCommand { get; set; }

        public List<Alumno> ListaAlumnos { get; set; }

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


    }
}
