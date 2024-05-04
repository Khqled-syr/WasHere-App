using System.Windows;
using System.Windows.Controls;
using WasHere.Database;
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


            ClearOutput();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {

                SetOutput("Please enter all required fields.");
                LoginButton.IsEnabled = true;
                return;

            }


            using (var dbContext = new DatabaseContext())
            {
                Database.User? user = dbContext.Users.FirstOrDefault(user => user.UserName.ToLower() == username.ToLower());

                if (user != null)
                {
                    if(BC.Verify(PasswordBox.Password, user.Password))
                    {
                        // Authentication successful 
                        await Task.Delay(20);
                        SetOutput("Succesfully logged in!");
                        App.user = user;
                        await Task.Delay(2500);

                        MainUI mainUI = new MainUI();
                        mainUI.Show();
                        this.Close();
                    }
                    else
                    {
                        SetOutput("Password or Username is not correct, please try again.");
                        LoginButton.IsEnabled = true;
                        PasswordBox.Clear();
                        return;

                    }
                    LoginButton.IsEnabled = true;
                }
                else
                {
                    // Incorrect password
                    SetOutput("Password or Username is not correct, please try again.");
                    LoginButton.IsEnabled = true;
                    PasswordBox.Clear();
                    return;
                }
            }
            LoginButton.IsEnabled = true;
        }

        private void ClearOutput()
        {
            OutputTextBlock.Text = "";
            currentIndex = 0;
            outputText = "";
        }

        private async void SetOutput(string text)
        {
            ClearOutput();
            outputText = text;
            await TypeTextAsync();
        }

        private async void AppendOutput(string text)
        {
            outputText = text;
            await TypeTextAsync();
        }

        private async Task TypeTextAsync()
        {
            while (currentIndex < outputText.Length)
            {
                OutputTextBlock.Text += outputText[currentIndex];
                currentIndex++;
                await Task.Delay(50); // Adjust typing speed here
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
