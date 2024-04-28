using System.Configuration;
using System.Data;
using System.Windows;
using WasHere.Database;

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
