using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using WasHere.Database;
using WasHere.Utils;
using Windows.UI.Shell;
using BC = BCrypt.Net.BCrypt;
using WasHere.Settings;
using System.Windows.Controls;

namespace WasHere.ViewModel
{
    public partial class LoginUI : Window
    {

        private bool windowMovedByUser = false;

        public LoginUI()
        {
            InitializeComponent();
            CheckVpnOnStartUp();
            KeyAuthApi.KeyAuthApp.init();

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

            UserTextBox.Text = Settings.Settings.Default.LastUsername;
            PasswordBox.Password = Settings.Settings.Default.LastPassword;
            
            Settings.Settings.Default.Save();
        }


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UserTextBox.Text;
            var password = PasswordBox.Password;

            LoginButton.IsEnabled = false;

            Utils.OutputManager.ClearOutput(OutputTextBlock);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _ = Utils.OutputManager.SetOutputAsync(
                    OutputTextBlock,
                    "Please enter all required fields."
                );
                LoginButton.IsEnabled = true;
                return;
            }

            string userIP;

            try
            {
                userIP = await GetIPAddress.GetPublicIpAddressAsync();
            }
            catch (Exception ex)
            {
                _ = Utils.OutputManager.SetOutputAsync(
                     OutputTextBlock,
                     $"Unable to connect to the server, make sure you're connected to the internet!"
                 );
                LoginButton.IsEnabled = true;
                return;
            }

            using (var dbContext = new DatabaseContext())
            {
                var user = dbContext.Users.FirstOrDefault(user =>
                    user.UserName.ToLower() == username.ToLower()
                );

                if (user != null)
                {
                    if (BC.Verify(PasswordBox.Password, user.Password))
                    {
                        if (Utils.KeyAuthApi.KeyAuthApp.checkblack())
                        {
                            await Task.Delay(20);
                            _ = MessageBox.Show($"You are banned LOL!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            Close();
                            return;
                        }

                        string key = user.ActivationKey;

                        Utils.KeyAuthApi.KeyAuthApp.license(key.ToString());

                        if (Utils.KeyAuthApi.KeyAuthApp.response.success == true)
                        {
                            // Authentication successful
                            await Task.Delay(20);
                            _ = Utils.OutputManager.SetOutputAsync(
                                OutputTextBlock,
                                $"Succesfully logged in!"
                            );
                            Settings.Settings.Default.LastUsername = username;
                            Settings.Settings.Default.LastPassword = password;
                            Settings.Settings.Default.Save();

                            App.user = user;
                            await Task.Delay(2500);

                            var mainUI = new MainUI();
                            mainUI.Show();
                            Close();
                        }
                        else if (Utils.KeyAuthApi.KeyAuthApp.response.success == false)
                        {
                            // Authentication not successful
                            await Task.Delay(20);
                            _ = Utils.OutputManager.SetOutputAsync(
                                OutputTextBlock,
                                $"Activation key has been expired!"
                             );
                            LoginButton.IsEnabled = true;
                            return;
                        }
                    }
                    else
                    {
                        _ = Utils.OutputManager.SetOutputAsync(
                            OutputTextBlock,
                            "Password or Username is not correct, please try again."
                        );
                        LoginButton.IsEnabled = true;
                        PasswordBox.Clear();
                        return;
                    }

                    LoginButton.IsEnabled = true;
                }
                else
                {
                    // Incorrect password
                    _ = Utils.OutputManager.SetOutputAsync(
                        OutputTextBlock,
                        "Password or Username is not correct, please try again."
                    );
                    LoginButton.IsEnabled = true;
                    PasswordBox.Clear();
                    return;
                }
            }

            LoginButton.IsEnabled = true;
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
                    //_ = MessageBox.Show(errorMessage, "VPN or Cloudflare Warp Detected", MessageBoxButton.OK, MessageBoxImage.Error);
                    _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, errorMessage);
                    UserTextBox.IsEnabled = false;
                    PasswordBox.IsEnabled = false;
                    LoginButton.IsEnabled = false;
                    RegisterButton.IsEnabled = false;
                    await Task.Delay(5000);
                    Close();
                    return;
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Content = new RegistrationPage();
        }


        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            // Minimize the window
            WindowState = WindowState.Minimized;
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
        private void CloseAppBtn_Click(object sender, RoutedEventArgs e) => Close();
    }
}