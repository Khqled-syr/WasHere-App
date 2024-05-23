using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WasHere.Database;

namespace WasHere.Utils
{
    public class PermsChecker : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isAdmin;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static readonly string[] Admins = { "Khaled"}; // Example admin usernames

        public static bool IsCurrentUserAdmin()
        {
            string userName = GetCurrentUserName();
            return Admins.Contains(userName);
        }

        public static bool IsUserAdmin(string userName)
        {
            return Admins.Contains(userName);
        }

        private static string GetCurrentUserName()
        {
            return App.user?.UserName;
        }



        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                if (_isAdmin != value)
                {
                    _isAdmin = value;
                    OnPropertyChanged(nameof(IsAdmin));
                }
            }
        }
    }
}