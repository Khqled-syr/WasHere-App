using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using WasHere.Database;
using WasHere.Utils;

namespace WasHere.ViewModel
{

    public partial class RegistrationPage : Page
    {
        public static Api KeyAuthApp = new Api(
            name: "WasHere", // Application Name
            ownerid: "lYgBCNDiV1", // Owner ID
            secret: "f3a1cd3b7a23189aa43c700cba35f5413c8d30d84b2ed327d86d1a0e60ed6e87", // Application Secret
            version: "1.0" // Application Version, /*
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

                SumbitButton.IsEnabled = false;

                // Get user input from the text boxes
                string username = UsernameTextBox.Text;
                string password = PasswordBox.Password;
                string activationKey = KeyTextBox.Text;
                string ipAddress = await GetPublicIpAddressAsync();
                string pcName = Environment.MachineName;


                ClearOutput();


                // Check if IP address is null
                if (string.IsNullOrEmpty(ipAddress))
                {
                    SetOutput("Something unexpected went wrong. Please try again later.");
                    EnableSubmitButton();
                    return;
                }

                //VPN CHECKER
                bool isVpnUsed = await VpnChecker.IsVpnUsed(ipAddress);

                if (isVpnUsed || CloudflareChecker.IsCloudflareWarpEnabled())
                {
                    SetOutput("Please disable your VPN connection before registering.");
                    EnableSubmitButton();
                    return;
                }

                await Task.Run(() =>
                {
                    // Perform the initialization
                    KeyAuthApp.init();
                });


                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(activationKey))
                {

                    SetOutput("Please enter all required fields.");
                    EnableSubmitButton();
                    return;

                }
                KeyAuthApp.license(activationKey);

                if (!KeyAuthApp.response.success)
                {
                    SetOutput("Invalid activation key. Please check and try again.");
                    KeyTextBox.Clear();
                    EnableSubmitButton();
                    return;
                }
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
                        EnableSubmitButton();
                            return;
                        }

                        SetOutput($"Registration successful!\n\nUser: {newUser.UserName}\nPassword: {newUser.Password}");
                        await dbContext.AddUserAsync(newUser);
                        UsernameTextBox.Clear();
                        PasswordBox.Clear();
                        KeyTextBox.Clear();
                        SumbitButton.IsEnabled = false;
                    }
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetOutput("An error occurred. Please try again later.");
                EnableSubmitButton();
                return;
            }
        }

        private async void EnableSubmitButton()
        {
            // Delay for a short period before enabling the button to prevent spamming
            await Task.Delay(1000); // Adjust the delay time as needed
            SumbitButton.IsEnabled = true;
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
