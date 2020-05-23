using Prueba.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Prueba.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
    
        IMessage message = DependencyService.Get<IMessage>();
        List<Avisos> listaAvisos;
        Int32 idMaestro, idAlumno;
        String clavealumno;
        String mensaje;
        String nombreAlumno;

        public ChatViewModel(String Clave, Int32 idalumno, Int32 idmaestro)
        {
            DescargarAvisos(Clave);
            EnviarCommand = new Command(Enviar);
            idAlumno = idalumno;
            clavealumno = Clave;
            idMaestro = idmaestro;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public String NombreAlumno
        {
            get { return nombreAlumno; }
            set { nombreAlumno = value; Actualizar(); }
        }

        public List<Avisos> ListaAvisos
        {
            get { return listaAvisos; }
            set { listaAvisos = value; Actualizar(); }
        }

        public String Mensaje
        {
            get { return mensaje; }
            set { mensaje = value; Actualizar(); }
        }

        public Command EnviarCommand { get; set; }

        async Task DescargarAvisos(String Clave)
        {
            try
            {
                if (Connectivity.NetworkAccess==NetworkAccess.Internet)
                {
                    await App.MainAvisos.DescargarAvisos(Clave);
                }
                else
                {
                    message.ShowToast("Sin conexión a internet");
                }
                ListaAvisos = App.MainAvisos.GetBAvisosByAlumno(Clave);
            }
            catch (Exception ex)
            {
                message.ShowToast(ex.Message);
            }

        }

        private async void Enviar()
        {
            if (!String.IsNullOrWhiteSpace(Mensaje))
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    await App.MainAvisos.EnviarAvisos(1, 1, "prueba", DateTime.Today, DateTime.Today.AddMonths(6), "0001");
                    await DescargarAvisos(clavealumno);
                }
                else
                {
                    message.ShowToast("Sin conexión a internet");
                }
            }
            else
            {
                message.ShowToast("El contenido del mensaje no puede estar sin espacios en blanco o vacío");
            }
        }


        void Actualizar([CallerMemberName] String nam = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nam));
        }

    }
}
