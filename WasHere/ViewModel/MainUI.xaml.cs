using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WasHere.Database;
using WasHere.Utils;

namespace WasHere.ViewModel
{
    public partial class MainUI : Window
    {
        
        private PermsChecker _permsChecker;
        private DispatcherTimer _timer;
        private DateTime _lastUpdateTime;
        private bool windowMovedByUser = false;

        public MainUI()
        {
            InitializeComponent();
            OnLoaderStartUp();
            CheckVpnOnStartUp();
            CheckUserPermission();
            _permsChecker = new PermsChecker();

            // Set the initial WindowStartupLocation
            if (Settings.Settings.Default.IsFirstLaunch)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                Settings.Settings.Default.IsFirstLaunch = false;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Left = Settings.Settings.Default.WindowLeft;
                Top = Settings.Settings.Default.WindowTop;
            }

        }

        public void CheckUserPermission()
        {
            var viewModel = DataContext as PermsChecker;
            if (viewModel != null)
            {
                viewModel.IsAdmin = PermsChecker.IsCurrentUserAdmin();
            }
        }

        private async void CheckVpnOnStartUp()
        {
            while (true)
            {
                string ipAddress = await GetIPAddress.GetPublicIpAddressAsync();

                bool isVpnUsed = await VpnChecker.IsVpnUsed(ipAddress);

                if (isVpnUsed || CloudflareChecker.IsCloudflareWarpEnabled())
                {
                    string errorMessage = "Please disable your VPN for better experiance!";
                    _ = MessageBox.Show(errorMessage, "VPN or Cloudflare Warp Detected", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }
                await Task.Delay(TimeSpan.FromSeconds(1));

            }
        }

        private async void OnLoaderStartUp()
        {
            if (App.user != null)
            {
                using var dbContext = new DatabaseContext();
                UserTitle.Text = $"{App.user.UserName.ToUpper()}!";
                

                await Utils.OutputManager.SetOutputAsync(OutputTextBlock, GetFormattedMsg());

                ScheduleNextUpdate();
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void ScheduleNextUpdate()
        {
            var currentTime = DateTime.Now;
            var nextMinute = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0).AddMinutes(1);
            var timeUntilNextMinute = nextMinute - currentTime;

            _timer = new DispatcherTimer
            {
                Interval = timeUntilNextMinute
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Stop the current timer
            _timer.Stop();
            _timer.Tick -= Timer_Tick;

            // Update the UI
            OutputTextBlock.Text = GetFormattedMsg();

            // Schedule the next update
            ScheduleNextUpdate();
        }

        private string GetFormattedMsg()
        {
            return $"Welcome {App.user.UserName.ToUpper()}!\nLogged in at: {DateTime.Now.ToString("HH:mm tt")}\nLast Login: {KeyAuthApi.UnixTimeToString(long.Parse(KeyAuthApi.KeyAuthApp.user_data.lastlogin))}";
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _ = Utils.OutputManager.SetOutputAsync(
                OutputTextBlock,
                "Loading settings..."
                );

            await Task.Delay(2500);
            this.Content = new InformationsPage();
        }

        private async void SystemCommands_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                await Utils.SystemCommands.ClearSystemCache();
                _ = Utils.OutputManager.SetOutputAsync(
                    OutputTextBlock,
                    "All processes are done!"
                    );
            }
            catch (Exception ex)
            {
                _ = Utils.OutputManager.SetOutputAsync(
                    OutputTextBlock,
                    $"An error occurred: {ex.Message}"
                );
            }
        }

        private void Boostfps_Button(object sender, EventArgs e)
        {

        }

        private async void UsersButton_Click(object sender, RoutedEventArgs e)
        {
              UsersEditButton.IsEnabled = true;

            _ = Utils.OutputManager.SetOutputAsync(
                OutputTextBlock,
                "Loading Users Page..."
            );
            UsersEditButton.IsEnabled = false;
            await Task.Delay(2500);
            this.Content = new UsersEditPage();
        }

        private void CloseAppBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !windowMovedByUser)
            {
                windowMovedByUser = true;
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!windowMovedByUser)
            {
                // Only save the window position if it was moved by the user
                Settings.Settings.Default.WindowLeft = Left;
                Settings.Settings.Default.WindowTop = Top;
                Settings.Settings.Default.Save();
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop dragging the window
        }
    }
}
