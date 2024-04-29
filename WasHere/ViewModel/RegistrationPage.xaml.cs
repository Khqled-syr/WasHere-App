using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WasHere.Database;

namespace WasHere.ViewModel
{

    public partial class RegistrationPage : Page
    {
        public static Api KeyAuthApp = new Api(
            name: "WasHere", // Application Name
            ownerid: "6l9dTVjpxN", // Owner ID
            secret: "1d0e6bdb8f14135f0d681d1e52eb994b23d5b36533e34515b1125fa472217355",
            version: "1.0"
);


        private int currentIndex;
        private string? outputText;


        public RegistrationPage()
        {
            InitializeComponent();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                KeyAuthApp.init();

                // Get user input from the text boxes
                string username = UsernameTextBox.Text;
                string password = PasswordBox.Password;
                string activationKey = KeyTextBox.Text;
                string ipAddress = await GetPublicIpAddressAsync();
                string pcName = Environment.MachineName;

                ClearOutput();


                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(activationKey))
                { 

                    
                    SetOutput("Please enter all required fields.");
                    return;
                
                }

                KeyAuthApp.license(activationKey);           

                if (KeyAuthApp.response.success)
                {

                    User newUser = new User
                    {
                        UserName = username,
                        Password = password,
                        ActivationKey = activationKey,
                        IpAddress = ipAddress,
                        PcName = pcName
                    };

                    using (var dbContext = new DatabaseContext())
                    {
                        var existingUser = dbContext.Users.FirstOrDefault(u => u.UserName == username);

                        if (existingUser != null)
                        {
                            SetOutput("User already exists!");
                            return;
                        }

                        //MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        SetOutput($"Registration successful!\n\nUser: {newUser.UserName}\nPassword: {newUser.Password}");
                        await dbContext.AddUserAsync(newUser);
                        UsernameTextBox.Clear();
                        PasswordBox.Clear();
                        KeyTextBox.Clear();  
                    }
                }

                else
                {
                    //MessageBox.Show("Invalid activation key. Please check and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    SetOutput("Invalid activation key. Please check and try again.");
                    KeyTextBox.Clear();
                }
            }
            catch(Exception ex)
            {
                LogError(ex);
                //MessageBox.Show("An error occurred. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                SetOutput("An error occurred. Please try again later.");

            }
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



        private void LogError(Exception ex)
        {
            string logFilePath = "error.log";

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"Error occurred at {DateTime.Now}: {ex.Message}");
                    writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                    writer.WriteLine();
                }
            }
            catch (IOException)
            {
                // Failed to write to log file
                // Display a message to the user or handle the error as needed
            }
        }

        private async Task<string> GetPublicIpAddressAsync()
        {
            try
            {
                string ipAddress;
                using (var client = new HttpClient())
                {
                    ipAddress = await client.GetStringAsync("https://api64.ipify.org");
                }
                return ipAddress;
            }
            catch (Exception)
            {
                return "Error: Unable to retrieve public IP";
            }
        }

        private async void BackToLogin_Click(object sender, RoutedEventArgs e)
        {

            LoginUI loginUI = new LoginUI();
            loginUI.Visibility = Visibility.Visible;
            await Task.Delay(10);
            ((Window)Parent).Close();
        }

    }
}
