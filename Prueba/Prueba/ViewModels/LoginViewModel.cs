using Prueba.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Prueba.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        IMessage mensaje = DependencyService.Get<IMessage>();
        MainPage viewMain;
        private String clave;
        String password;
        Escuela escuela;
        List<Escuela> escuelas;
        private String errores;
        private Boolean cvwMensajeVisible;

        public LoginViewModel()
        {
            Descargar();
            InicarSesionCommand = new Command(IniciarSesion);
            DescargarEscuelasCommand = new Command(Descargar);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public Command InicarSesionCommand { get; set; }
        public Command DescargarEscuelasCommand { get; set; }

        public Boolean CvwMensajeVisisble
        {
            get { return cvwMensajeVisible; }
            set { cvwMensajeVisible = value;Actualizar(); }
        }

        public String Errores
        {
            get { return errores; }
            set { errores = value; Actualizar(); }
        }


        public String Clave
        {
            get { return clave; }
            set { clave = value; Actualizar(); }
        }

        public String Password
        {
            get { return password; }
            set { password = value; Actualizar(); }
        }

        public Escuela Escuela
        {
            get { return escuela; }
            set { escuela = value; Actualizar(); }
        }

        public List<Escuela> ListaEscuelas
        {
            get { return escuelas; }
            set { escuelas = value; Actualizar(); }
        }

       


        private async void Descargar()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await App.MainAvisos.DescargarEscuelas();
                CvwMensajeVisisble = false;
            }
            else
            {
                
                mensaje.ShowToast("Sin conexión a internet");
                
                CvwMensajeVisisble = true;
            }
            ListaEscuelas = App.MainAvisos.GetEscuelas();
        }

        private async void DescargarAvisos(string nombreescuela) 
        {
            if (Connectivity.NetworkAccess==NetworkAccess.Internet)
            {
                await App.MainAvisos.DescargarAvisosGenerales(nombreescuela);
            }
        } 

        private async void IniciarSesion(object obj)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (Validar())
                {
                    try
                    {
                        Boolean resultado = await App.MainAvisos.IniciarSesion(Clave, Password, Escuela.IdEscuela.ToString());
                        if (resultado)
                        {
                            if (viewMain == null)
                            {
                                viewMain = new MainPage();
                            }
                            DescargarAvisos(Escuela.NombreEscuela);
                            Maestro maestro = App.MainAvisos.GetMaestro(Clave);
                            MainPageViewModel mainPageViewModel = new MainPageViewModel(maestro);
                            viewMain.BindingContext = mainPageViewModel;
                            Application.Current.MainPage = new NavigationPage(viewMain);
                        }
                        else
                        {
                            Errores += "No ha sido posible iniciar sesión";
                        }
                    }
                    catch (ArgumentException ae)
                    {
                        Errores += ae.Message;
                    }
                }
            }
            else
            {
                mensaje.ShowToast("Sin conexión a internet");
            }

        }


        Boolean Validar()
        {
            Errores = "";
            if (String.IsNullOrWhiteSpace(Clave))
            {
                Errores += "La clave no puede ir vacía o tener espacios blanco\n";
            }
            if (String.IsNullOrWhiteSpace(Password))
            {
                Errores += "El password no puede ir vacío o tener espacios en blanco\n";
            }
            if (Escuela == null)
            {
                Errores += "La escuela no puede estar vacía\n";
            }

            return Errores.Count() == 0;
        }

        void Actualizar([CallerMemberName]string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

    }
}
