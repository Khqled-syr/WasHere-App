using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WasHere.Database;

namespace WasHere.ViewModel
{

    public partial class MainUI : Window
    {
        public MainUI()
        {
            InitializeComponent();
            OnStart();
        }

        public void OnStart()
        {

            if (App.user != null)
            {
                using (var DbContext = new DatabaseContext())
                {
                    UserTitle.Text = $"Logged in as: {App.user.UserName}";
                }

            }
            else
            {
                MessageBox.Show("NULL");
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop dragging the window
        }


        private void CloseAppBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new SettingsPage();
        }

        private void Clear_dns_cache_button(object sender, RoutedEventArgs e)
        {
            string dns = "Resources/dns.cmd";
            string cache = "Resources/cache.cmd";
            string logs = "Resources/logs.cmd";
            string tempfiles = "Resources/tempfiles.cmd";
            string diskcleanup = "Resources/diskclean.cmd";

            List<string> missingFiles = new List<string>();

            if (!System.IO.File.Exists(dns))
                missingFiles.Add("dns.cmd");

            if (!System.IO.File.Exists(cache))
                missingFiles.Add("cache.cmd");

            if (!System.IO.File.Exists(logs))
                missingFiles.Add("logs.cmd");

            if (!System.IO.File.Exists(tempfiles))
                missingFiles.Add("tempfiles.cmd");

            if (!System.IO.File.Exists(diskcleanup))
                missingFiles.Add("diskclean.cmd");

            if (missingFiles.Count > 0)
            {
                string errorMessage = "The following files are missing:\n" + string.Join("\n", missingFiles);
                MessageBox.Show(errorMessage);
            }
            else
            {
                Task.Run(async () =>
                {
                    await Task.WhenAll(
                        Process.Start(dns).WaitForExitAsync(),
                        Process.Start(cache).WaitForExitAsync(),
                        Process.Start(logs).WaitForExitAsync(),
                        Process.Start(tempfiles).WaitForExitAsync(),
                        Process.Start(diskcleanup).WaitForExitAsync()
                        );

                    MessageBox.Show("All processes are done!");
                });
            }
        }

        private void Boostfps_Button(object sender, EventArgs e)
        {
            BoostFps();
        }


        private void BoostFps()
        {
            MessageBox.Show("Performance boosting actions completed.", "Performance Boost", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}