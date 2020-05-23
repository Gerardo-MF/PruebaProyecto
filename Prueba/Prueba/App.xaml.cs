using Prueba.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prueba.Models;
using Prueba.ViewModels;

namespace Prueba
{
    public partial class App : Application
    {
        public static String PersonalFolder { get; set; } =$"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}";
        public static MainAvisos MainAvisos { get; set; } = new MainAvisos();
        public App()
        {
            InitializeComponent();

             MainPage = new ventanaprueba123();
            //MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {

        }
    }
}
