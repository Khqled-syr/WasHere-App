using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasHere.Utils
{
    public static class VpnChecker
    {

        private static string[] KnownVPNIPs =
    {
        "1.2.3.4", // Add more known VPN IP addresses here
        "5.6.7.8"
    };


        public static async Task<bool> IsVpnUsed(string ipAddress)
        {

            if (IsLocalIpAddress(ipAddress)) {
                return false;
            }

            if(KnownVPNIPs.Contains(ipAddress))
            {
                return true;
            }

            if (IsVpnIpAddress(ipAddress))
            {
                return true;
            }


            return false;
        }

        private static bool IsLocalIpAddress(string ipAddress)
        {
            return ipAddress.StartsWith("192.168.") || ipAddress.StartsWith("10.") || ipAddress == "127.0.0.1";
        }

        private static bool IsVpnIpAddress(string ipAddress)
        {
            // Check if the IP address belongs to a VPN range
            // You can add your own logic here based on known VPN IP address ranges
            // Example:
            // if (ipAddress.StartsWith("172.16.") || ipAddress.StartsWith("172.17.") || ipAddress.StartsWith("172.18.") || ipAddress.StartsWith("172.19."))
            // {
            //     return true;
            // }
            return false;
        }

    }
}
