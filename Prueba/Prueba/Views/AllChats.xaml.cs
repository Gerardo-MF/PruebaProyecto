using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prueba.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllChats : ContentPage
    {
        public AllChats()
        {
            InitializeComponent();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PopAsync();
        }

        private void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            txtMensaje.Text = "";
        }
    }
}