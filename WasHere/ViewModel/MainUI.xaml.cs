using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WasHere.Database;

namespace WasHere.ViewModel
{

    public partial class MainUI : Window
    {
        public MainUI()
        {
            InitializeComponent();
            OnStart();
        }

        public void OnStart()
        {

            if (App.user != null)
            {
                using (var DbContext = new DatabaseContext())
                {
                    UserTitle.Text = $"{App.user.UserName}!";
                    Utils.OutputManager.SetOutputAsync(OutputTextBlock, $"Welcome {App.user.UserName}");
                }
            }
            else
            {
                MessageBox.Show("NULL");
            }
        }


        private async void AccountButton(object sender, RoutedEventArgs e)
        {
            Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Loading Account settings...");
            await Task.Delay(2500);
            this.Content = new SettingsPage();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop dragging the window
        }


        private void CloseAppBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Loading settings...");
            await Task.Delay(2500);
            this.Content = new SettingsPage();
        }


        private async void SystemCommands_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                await Utils.SystemCommands.ClearSystemCache();
                Utils.OutputManager.SetOutputAsync(OutputTextBlock, "All processes are done!");
            }
            catch (Exception ex)
            {
                Utils.OutputManager.SetOutputAsync(OutputTextBlock, $"An error occurred: {ex.Message}");
            }
        }

        private void Boostfps_Button(object sender, EventArgs e)
        {
            BoostFps();
        }


        private void BoostFps()
        {
            Utils.OutputManager.SetOutputAsync(OutputTextBlock, "Performance boosting actions completed.");
        }
    }
}