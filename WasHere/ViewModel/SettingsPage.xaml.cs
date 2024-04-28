using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WasHere.ViewModel
{
    public partial class SettingsPage : Page
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public SettingsPage()
        {
            InitializeComponent();
            Loaded += SettingsPage_Loaded;
        }

        private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Load data asynchronously
            var systemInfo = await LoadSystemInfoAsync();

            // Set DataContext
            DataContext = systemInfo;
        }

        private async Task<SystemInfo> LoadSystemInfoAsync()
        {
            // Simulate loading data
            await Task.Delay(2000);

            // Return loaded data
            return new SystemInfo();
        }
        private async void BackToMainUI_Click(object sender, RoutedEventArgs e)
        {
            MainUI mainUI = new MainUI();
            mainUI.Visibility = Visibility.Visible;
            await Task.Delay(10);
            System.Windows.Window win = (System.Windows.Window)Parent;
            win.Close();
        }

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
