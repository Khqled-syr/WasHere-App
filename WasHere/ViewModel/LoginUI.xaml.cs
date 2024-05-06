using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using WasHere.Database;
using WasHere.Utils;
using BC = BCrypt.Net.BCrypt;

namespace WasHere.ViewModel
{
    public partial class LoginUI : Window
    {
        public LoginUI()
        {
            InitializeComponent();
            CheckVpnOnStartUp();
            KeyAuthApi.KeyAuthApp.init();

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
                userIP = await GetPublicIpAddressAsync();
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
                            _ = Utils.OutputManager.SetOutputAsync(
                                OutputTextBlock,
                                $"You have been banned!");
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
                string ipAddress = await GetPublicIpAddressAsync();

                bool isVpnUsed = await VpnChecker.IsVpnUsed(ipAddress);

                if (isVpnUsed || CloudflareChecker.IsCloudflareWarpEnabled())
                {
                    string errorMessage = "Please disable your VPN or Cloudflare Warp before logging in.";
                    _ = MessageBox.Show(errorMessage, "VPN or Cloudflare Warp Detected", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        private async Task<string> GetPublicIpAddressAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Use the ipinfo.io API to fetch the public IP address
                    return await client.GetStringAsync("https://ipinfo.io/ip");
                }
                catch (Exception ex)
                {
                    // Handle exceptions, such as network errors
                    return $"Error: {ex.Message}";
                }
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop dragging the window
        }
        private void CloseAppBtn_Click(object sender, RoutedEventArgs e) => Close();
    }
}