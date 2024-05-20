using System.Linq;
using WasHere.Database;

namespace WasHere.Utils
{
    public static class PermsChecker
    {
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
    }
}