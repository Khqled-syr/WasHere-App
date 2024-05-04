using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using WasHere.Database;
using Windows.Media.Protection.PlayReady;
using BC = BCrypt.Net.BCrypt;

namespace WasHere.ViewModel
{
    public partial class LoginUI : Window
    {

        private int currentIndex;
        private string? outputText;

        public LoginUI()
        {
            InitializeComponent();
        }


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserTextBox.Text;
            string password = PasswordBox.Password;

            LoginButton.IsEnabled = false;

            Utils.OutputManager.ClearOutput(OutputTextBlock);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {

                Utils.OutputManager.SetOutputAsync(OutputTextBlock,"Please enter all required fields.");
                LoginButton.IsEnabled = true;
                return;
            }

            string userIP;
            try
            {
                userIP = GetPublicIpAddress();
            }
            catch (Exception ex)
            {
                Utils.OutputManager.SetOutputAsync(OutputTextBlock, $"Unable to connect to the server, make sure you're connected to the internet!");
                LoginButton.IsEnabled = true;
                return;
            }


            using (var dbContext = new DatabaseContext())
            {
                Database.User? user = dbContext.Users.FirstOrDefault(user => user.UserName.ToLower() == username.ToLower());

                if (user != null)
                {
                    if (BC.Verify(PasswordBox.Password, user.Password))
                    {
                        if (userIP != user.IpAddress)
                        {
                            Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Please disable your vpn before logging in!");
                            LoginButton.IsEnabled = true;
                            return;
                        }

                        // Authentication successful 
                        await Task.Delay(20);
                        Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Succesfully logged in!");
                        App.user = user;
                        await Task.Delay(2500);

                        MainUI mainUI = new MainUI();
                        mainUI.Show();
                        this.Close();
                    }
                    else
                    {
                        Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Password or Username is not correct, please try again.");
                        LoginButton.IsEnabled = true;
                        PasswordBox.Clear();
                        return;

                    }
                    LoginButton.IsEnabled = true;
                }
                else
                {
                    // Incorrect password
                    Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Password or Username is not correct, please try again.");
                    LoginButton.IsEnabled = true;
                    PasswordBox.Clear();
                    return;
                }
            }
            LoginButton.IsEnabled = true;
        }

        private string GetPublicIpAddress()
        {
            using (var client = new WebClient())
            {
                return client.DownloadString("https://api64.ipify.org");
            }
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new RegistrationPage();
        }



        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            // Minimize the window
            this.WindowState = WindowState.Minimized;
        }

        private void CloseAppBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
