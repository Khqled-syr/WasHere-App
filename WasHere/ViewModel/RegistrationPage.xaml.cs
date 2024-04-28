using System.Windows;
using System.Windows.Controls;
using WasHere.Database;

namespace WasHere.ViewModel
{

    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Get user input from the text boxes
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string activationKey = KeyTextBox.Text;
            //string ipAddress = GetIpAddress(); // You need to implement this method
            //string pcName = GetPcName(); // You need to implement this method

            // Create a new User object
            User newUser = new User
            {
                UserName = username,
                Password = password,
                ActivationKey = activationKey,
                //IpAddress = ipAddress,
                //PcName = pcName
            };

            // Add the new user to the database
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    dbContext.Users.Add(newUser);
                    await dbContext.SaveChangesAsync();
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    UsernameTextBox.Clear();
                    PasswordBox.Clear();
                    KeyTextBox.Clear();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Check and display inner exceptions
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    MessageBox.Show($"Inner Exception: {ex.Message}", "Inner Exception", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private async void BackToLogin_Click(object sender, RoutedEventArgs e)
        {

            LoginUI loginUI = new LoginUI();
            loginUI.Visibility = Visibility.Visible;
            await Task.Delay(10);
            System.Windows.Window win = (System.Windows.Window)Parent;
            win.Close();
        }



    }
}
