using System.Net.NetworkInformation;

namespace WasHere.Utils
{
    public static class CloudflareChecker
    {
        public static bool IsCloudflareWarpEnabled()
        {
            // Check if Cloudflare WARP VPN interface exists
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            return networkInterfaces.Any(
                ni => ni.Description.Contains("Cloudflare Warp", StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}