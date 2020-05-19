using Prueba.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Prueba.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //List<Avisos> listaAvisos;

        public ChatViewModel(String Clave)
        {
            DescargarAvisos(Clave);
            ListaAvisos = App.MainAvisos.GetBAvisosByAlumno(Clave);
        }

        public List<Avisos> ListaAvisos { get; set; }

        //public List<Avisos> ListaAvisos
        //{
        //    get { return listaAvisos; }
        //    set { listaAvisos = value; Actualizar(); }
        //}
        async void DescargarAvisos(String Clave) 
        {
            try
            {
                await App.MainAvisos.DescargarAvisos(Clave);
            }
            catch (Exception ex)
            {
                IMessage mensaje = DependencyService.Get<IMessage>();
                mensaje.ShowToast(ex.Message);
            }

        }

        void Actualizar([CallerMemberName] String nam = "") 
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nam));
        }

    }
}
