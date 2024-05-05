using System.Collections.Generic;
using System.Linq;

namespace WasHere.Utils
{
    public static class VpnChecker
    {
        private static HashSet<string> KnownVPNIPs = new HashSet<string>
        {
            "1.2.3.4", // Add more known VPN IP addresses here
            "5.6.7.8"
        };

        public static async Task<bool> IsVpnUsed(string ipAddress)
        {
            if (IsLocalIpAddress(ipAddress))
            {
                return false;
            }

            if (KnownVPNIPs.Contains(ipAddress))
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
            // Add logic to detect VPN IP addresses based on known VPN ranges or patterns
            // Example:
            // if (ipAddress.StartsWith("172.16.") || ipAddress.StartsWith("172.17.") || ipAddress.StartsWith("172.18.") || ipAddress.StartsWith("172.19."))
            // {
            //     return true;
            // }

            return false;
        }
    }
}
