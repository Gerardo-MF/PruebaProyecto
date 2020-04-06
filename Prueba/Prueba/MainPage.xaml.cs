using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prueba
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            asd.Source = ImageSource.FromUri(new Uri("https://cdn2.iconfinder.com/data/icons/user-icon-2-1/100/user_5-15-512.png"));
            xyz.Source = ImageSource.FromUri(new Uri("https://www.itesrc.edu.mx/logos/logotec%20(weld).jpg"));
        }

      
    }
}
