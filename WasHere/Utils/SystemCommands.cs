using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WasHere.Utils
{
    public static class SystemCommands
    {
        public static async Task ClearSystemCache()
        {
            List<string> commands = new List<string>
            {
                "ipconfig /flushdns",
                "del /q /s %TEMP%",
                "wevtutil.exe cl Application && wevtutil.exe cl System && wevtutil.exe cl Security",
                "del /q /s %TEMP%",
                "cleanmgr /sagerun:1"
            };

            await ExecuteCommandsAsync(commands);
        }

        private static async Task ExecuteCommandsAsync(IEnumerable<string> commands)
        {
            foreach (var command in commands)
            {
                await ExecuteCommandAsync(command);
            }

            Console.WriteLine("All processes are done!");
        }

        private static async Task ExecuteCommandAsync(string command)
        {
            await Task.Run(() =>
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (Process process = new Process())
                {
                    process.StartInfo = processInfo;

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine($"Output: {e.Data}");
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine($"Error: {e.Data}");
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
            });
        }
    }
}