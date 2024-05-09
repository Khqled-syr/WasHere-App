using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WasHere.Database;
using WasHere.Utils;
using BC = BCrypt.Net.BCrypt;


namespace WasHere.ViewModel
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            // Perform the initialization
            InitializeComponent();
            Utils.KeyAuthApi.KeyAuthApp.init();
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

                Utils.OutputManager.ClearOutput(OutputTextBlock);


                // Check if IP address is null
                if (string.IsNullOrEmpty(ipAddress))
                {
                    _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Something unexpected went wrong. Please try again later.");
                    EnableSubmitButton();
                    return;
                }

                //VPN CHECKER
                bool isVpnUsed = await VpnChecker.IsVpnUsed(ipAddress);

                if (isVpnUsed || CloudflareChecker.IsCloudflareWarpEnabled())
                { 
                    _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Please disable your VPN connection before registering.");
                    EnableSubmitButton(); 
                    return;
                }


                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(activationKey))
                {
                    _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Please enter all required fields.");
                    EnableSubmitButton();
                    return;
                }
                if (password.Length < 8)
                {
                    _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Password must be at least 8 characters long.");
                    EnableSubmitButton();
                    return;
                }
                // Check if password is compromised
                if (await IsPasswordCompromised(password))
                {
                    _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Password has been compromised. Please choose a different one.");
                    EnableSubmitButton();
                    return;
                }
                



                Utils.KeyAuthApi.KeyAuthApp.license(activationKey);

                if (!Utils.KeyAuthApi.KeyAuthApp.response.success)
                {
                    _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Invalid activation key. Please check and try again.");
                    KeyTextBox.Clear();
                    EnableSubmitButton();
                    return;
                }


                var hashed = BC.HashPassword(PasswordBox.Password);
                User newUser = new User
                {
                    UserName = username,
                    Password = hashed,
                    ActivationKey = activationKey,
                    IpAddress = ipAddress,
                    PcName = pcName

                };

                using (var dbContext = new DatabaseContext())
                {
                    var existingUser = dbContext.Users.FirstOrDefault(u => u.UserName == username);

                    if (existingUser != null)
                    {
                        _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, "User already exists!");
                            EnableSubmitButton();       
                            return;
                        }

                    _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, $"Registration successful!\n\nUsername: {newUser.UserName}\nPassword: {PasswordBox.Password}");
                    await dbContext.AddUserAsync(newUser);
                    UsernameTextBox.Clear();
                    PasswordBox.Clear();
                    KeyTextBox.Clear();
                    SumbitButton.IsEnabled = false;
                    await Task.Delay(3500);
                    LoginUI loginUI = new LoginUI();
                    loginUI.Visibility = Visibility.Visible;
                    await Task.Delay(10);
                    ((Window)Parent).Close(); 
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                _ = Utils.OutputManager.SetOutputAsync(OutputTextBlock, "An error occurred. Please try again later.");
                EnableSubmitButton();
                return;
            }
        }

        private async Task<bool> IsPasswordCompromised(string password)
        {
            // Use "Have I Been Pwned" API to check if the password is compromised
            // Example using HttpClient:
            HttpClient client = new HttpClient();
            string apiUrl = "https://api.pwnedpasswords.com/range/";
            string hash = CalculateSha1Hash(password);
            string prefix = hash.Substring(0, 5);
            string suffix = hash.Substring(5);
            HttpResponseMessage response = await client.GetAsync(apiUrl + prefix);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent.Contains(suffix.ToUpper());
            }
            else
            {
                // Handle API error
                return false;
            }
        }
        private string CalculateSha1Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha1.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hash)
                {
                    stringBuilder.Append(b.ToString("X2"));
                }
                return stringBuilder.ToString();
            }
        }
        private async void EnableSubmitButton()
        {
            // Delay for a short period before enabling the button to prevent spamming
            await Task.Delay(1000); // Adjust the delay time as needed
            SumbitButton.IsEnabled = true;
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