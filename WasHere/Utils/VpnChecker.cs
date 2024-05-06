using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WasHere.Utils
{
    public static class VpnChecker
    {
        private static HashSet<string> KnownVPNIPs = new HashSet<string>
        {
            "1.2.3.4", // Add more known VPN IP addresses here
            "5.6.7.8",
            // Add more known VPN IP addresses or ranges here
            // Example: "10.8.", "172.16.", etc.
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

            if (await IsVpnIpAddress(ipAddress))
            {
                return true;
            }

            return false;
        }

        private static bool IsLocalIpAddress(string ipAddress)
        {
            return ipAddress.StartsWith("192.168.") || ipAddress.StartsWith("10.") || ipAddress == "127.0.0.1";
        }

        private static async Task<bool> IsVpnIpAddress(string ipAddress)
        {
            try
            {
                // Perform asynchronous DNS lookup to resolve domain name
                IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ipAddress);
                string domainName = hostEntry.HostName;

                // Add logic to check if the domain name corresponds to a known VPN service
                if (IsKnownVpnDomain(domainName))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Handle DNS lookup errors
                Console.WriteLine($"Error performing DNS lookup: {ex.Message}");
            }

            return false;
        }

        private static bool IsKnownVpnDomain(string domainName)
        {
            // Add logic to check if the domain name corresponds to a known VPN service
            // Example: Check if the domain name contains keywords associated with VPN services
            return domainName.Contains("vpn") || domainName.Contains("expressvpn") || domainName.Contains("nordvpn");
        }
    }
}
