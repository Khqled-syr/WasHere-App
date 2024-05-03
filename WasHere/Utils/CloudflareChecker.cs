using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasHere.Utils
{
    public static class CloudflareChecker
    {
        public static bool IsCloudflareWarpEnabled()
        {
            // Check if Cloudflare WARP is running
            string[] processNames = { "cloudflarewarp", "warp", "Cloudflare WARP" };

            foreach(var processName in processNames)
            {
                Process[] processes = Process.GetProcessesByName(processName);
                if(processes.Length > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
