using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
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



                OutputTextBlock.Text = ""; // Clear previous messages


                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(activationKey))
                { 

                    
                    OutputTextBlock.Text += "Please enter all required fields.";
                    await Task.Delay(2500);
                    OutputTextBlock.Text = "";
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
                            OutputTextBlock.Text += "User already exists!";
                            await Task.Delay(2500);
                            OutputTextBlock.Text = "";
                            return;
                        }


                        //MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        OutputTextBlock.Text += "Registration successful!\n";
                        await dbContext.AddUserAsync(newUser);
                        await Task.Delay(5000);
                        OutputTextBlock.Text = "";
                        OutputTextBlock.Text += $"User: {newUser.UserName}\n";
                        OutputTextBlock.Text += $"Password: {newUser.Password}";
                        await Task.Delay(3000);
                        OutputTextBlock.Text = "";
                        UsernameTextBox.Clear();
                        PasswordBox.Clear();
                        KeyTextBox.Clear();  
                    }
                }

                else
                {
                    //MessageBox.Show("Invalid activation key. Please check and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    OutputTextBlock.Text += "Invalid activation key. Please check and try again.";
                    KeyTextBox.Clear();
                    await Task.Delay(2500);
                    OutputTextBlock.Text = "";
                }
            }
            catch(Exception ex)
            {
                LogError(ex);
                //MessageBox.Show("An error occurred. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                OutputTextBlock.Text += "An error occurred. Please try again later.";
                await Task.Delay(2500);
                OutputTextBlock.Text = "";

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
