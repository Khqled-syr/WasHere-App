using Newtonsoft.Json.Converters;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using WasHere.Database;
using BC = BCrypt.Net.BCrypt;

namespace WasHere.ViewModel
{
    public partial class LoginUI : Window
    {
        int currentIndex;
        string? outputText;


        public static Api KeyAuthApp = new Api(
            name: "WasHere", // Application Name
            ownerid: "lYgBCNDiV1", // Owner ID
            secret: "f3a1cd3b7a23189aa43c700cba35f5413c8d30d84b2ed327d86d1a0e60ed6e87", // Application Secret
            version: "1.0" // Application Version, /*
);


        public LoginUI()
        {
            InitializeComponent();

             KeyAuthApp.init();

        }

    async void LoginButton_Click(object sender, RoutedEventArgs e)
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

                        if (KeyAuthApp.checkblack())
                        {
                            await Task.Delay(20);
                            _ = Utils.OutputManager.SetOutputAsync(
                                OutputTextBlock,
                                $"You have been banned!");
                            return;
                        }

                        string key = user.ActivationKey;

                        KeyAuthApp.license(key.ToString());

                        if (KeyAuthApp.response.success == true)
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
                        else if(KeyAuthApp.response.success == false)
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


        private DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);

            try
            {
                dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            }
            catch
            {
                dtDateTime = DateTime.MaxValue;
            }
            return dtDateTime;
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



        void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Content = new RegistrationPage();
        }

        void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            // Minimize the window
            WindowState = WindowState.Minimized;
        }

        void CloseAppBtn_Click(object sender, RoutedEventArgs e) => Close();
    }
}
