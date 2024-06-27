using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WasHere.Utils;

namespace WasHere.ViewModel
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new PermsChecker();
        }

        private void SystemCommands_Button(object sender, RoutedEventArgs e)
        {
            // Implement your system optimization logic here
            OutputTextBlock.Text = "System optimization started...";
        }

        private void Boostfps_Button(object sender, RoutedEventArgs e)
        {
            // Implement your FPS boost logic here
            OutputTextBlock.Text = "FPS boost initiated...";
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement user edit logic here
            OutputTextBlock.Text = "User edit button clicked...";
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement settings navigation logic here
            OutputTextBlock.Text = "Navigating to settings...";
            // Example: Navigate to settings page
            // NavigationService.Navigate(new SettingsPage());
        }

    }
}
