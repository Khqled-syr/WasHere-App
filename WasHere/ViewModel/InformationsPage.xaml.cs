using System.Windows;
using System.Windows.Controls;

namespace WasHere.ViewModel
{
    public partial class InformationsPage : Page
    {
        private Informations info; // Instance of the Info class

        public InformationsPage()
        {
            InitializeComponent();
            Loaded += InformationsPage_Loaded;
            info = new Informations(); // Initialize an instance of the Info class
        }

        private void InformationsPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the DataContext of the SettingsPage to the instance of the Info class
            DataContext = info;
        }

        // Handle the event when the user clicks on the "Back to Main UI" button
        private async void BackToMainUI_Click(object sender, RoutedEventArgs e)
        {
            MainUI mainUI = new MainUI();
            mainUI.Visibility = Visibility.Visible;
            await Task.Delay(10);
            System.Windows.Window win = (System.Windows.Window)Parent;
            win.Close();
        }

        // Handle the event when the user clicks on the "Logout" button
        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginUI loginUI = new LoginUI();
            loginUI.Visibility = Visibility.Visible;
            await Task.Delay(10);
            System.Windows.Window win = (System.Windows.Window)Parent;
            win.Close();
        }
    }
}