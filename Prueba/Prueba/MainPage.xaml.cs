using Prueba.Views;
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
    public partial class MainPage :ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
 
        }

        private  void chatsboton_Clicked(object sender, EventArgs e)
        {
            AllChats all = new AllChats();
            Navigation.PushAsync(all);
           
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Chat c = new  Chat();
            Navigation.PushAsync(c);
                  
        }
    }
}
