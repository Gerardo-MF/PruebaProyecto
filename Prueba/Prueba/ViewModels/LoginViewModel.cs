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
    public class LoginViewModel:INotifyPropertyChanged
    {
        Maestro maestro;
        MainPage viewMain;
        private String clave;
        String password;
        Escuela escuela;
        List<AvisosGenerales> Avisos;
        List<Escuela> escuelas;
        private String errores;

        public LoginViewModel(Escuela l)
        {
          
            Descargar();
            DescargarAvisos(l);
            maestro = new Maestro();
            InicarSesionCommand = new Command(IniciarSesion);

        }


        public event PropertyChangedEventHandler PropertyChanged;

        //public List<Escuela> ListaEscuelas { get; set; }
        public Command InicarSesionCommand { get; set; }



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

        public Maestro Maestro
        {
            get { return maestro; }
            set { maestro = value; Actualizar(); }
        }

        public List<Escuela> ListaEscuelas
        {
            get { return escuelas; }
            set { escuelas = value; Actualizar(); }
        }

        public AvisosGenerales Aviso
        {
            get { return aviso; }
            set { aviso = value; Actualizar(); }
        }

        public List<AvisosGenerales> ListaAvisos
        {
            get { return Avisos; }
            set { Avisos = value; Actualizar(); }
        }

        void Actualizar([CallerMemberName]string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        private async void Descargar()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await App.MainAvisos.DescargarEscuelas();
            }
            ListaEscuelas = App.MainAvisos.GetEscuelas();
        }

        public async void DescargarAvisos(Escuela NombreEscuela)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await App.MainAvisos.DescargarAvisosGenerales(NombreEscuela.NombreEscuela);
            }
            ListaAvisos = App.MainAvisos.GetAvisosGenerales();
        }


        private async void IniciarSesion(object obj)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (Validar())
                {
                    var resultado = await App.MainAvisos.IniciarSesion(Clave, Password, Escuela.IdEscuela.ToString());
                    if (resultado)
                    {
                        if (viewMain == null)
                        {
                            viewMain = new MainPage();
                        }
                        maestro = App.MainAvisos.GetMaestro();
                        MainPageViewModel mainPageViewModel = new MainPageViewModel(maestro);
                        viewMain.BindingContext = mainPageViewModel;
                        Application.Current.MainPage = new NavigationPage(viewMain);
                    }
                }
            }
            else
            {
                IMessage mensaje = DependencyService.Get<IMessage>();
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
            if (Escuela==null)
            {
                Errores += "La escuela no puede estar vacía\n";
            }

            return Errores.Count() == 0;
        }

    }
}
