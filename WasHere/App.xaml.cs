using System;
using System.Windows;
using System.Windows.Controls;
using WasHere.Database;
using WasHere.ViewModel;
using Windows.ApplicationModel.Activation;
using Windows.UI;

namespace WasHere
{

    public partial class App : Application
    {
        public static User? user;

        public App()
        {
            ShutdownMode = ShutdownMode.OnLastWindowClose;
        }
    }

}
