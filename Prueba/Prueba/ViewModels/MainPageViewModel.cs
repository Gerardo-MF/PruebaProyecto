using Prueba.Models;
using System;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Prueba.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Maestro m;

        public MainPageViewModel(Maestro maestro)
        {
            Descargar(maestro);
            ListaAiumnos = App.MainAvisos.GetGrupoAlumnos();
            VerChatCommand = new Command(VerChat);
        }

        public Command VerChatCommand { get; set; }

        public List<Alumno> ListaAiumnos { get; set; }

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

        private void VerChat(object obj)
        {
            throw new NotImplementedException();
        }


    }
}
