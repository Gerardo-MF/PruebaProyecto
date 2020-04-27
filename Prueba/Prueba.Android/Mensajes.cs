using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(Prueba.Droid.Mensajes))]
namespace Prueba.Droid
{
    public class Mensajes : IMessage
    {
        public void ShowToast(String text)
        {
            var toast = Toast.MakeText(Application.Context, text, ToastLength.Long);
            toast.Show();
        }

    }
}