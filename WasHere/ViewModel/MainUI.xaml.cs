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
        
        private Status _status;
        private DispatcherTimer _timer;
        private DateTime _lastUpdateTime;

        public MainUI()
        {
            InitializeComponent();
            OnLoaderStartUp();
            CheckVpnOnStartUp();
            CheckUserPermission();
            _status = new Status();
        }

        public void CheckUserPermission()
        {
            var viewModel = DataContext as Status;
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

                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _timer.Tick += Timer_Tick;
                _timer.Start();
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var currentTime = DateTime.Now;
            if (currentTime.Second != _lastUpdateTime.Second)
            {
                OutputTextBlock.Text = GetFormattedMsg();
                _lastUpdateTime = currentTime;
            }
        }

        private string GetFormattedMsg()
        {
            return $"Welcome {App.user.UserName.ToUpper()}!\n{DateTime.Now.ToString()}";
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
                await _status.StartOptimizationProcess();
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop dragging the window
        }
    }
}
