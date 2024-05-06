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
            OnLoaderStartUp();
        }
        private void OnLoaderStartUp()
        {
            if (App.user != null)
            {
                using (var DbContext = new DatabaseContext())
                {
                    UserTitle.Text = $"{App.user.UserName}!";
                    _ = Utils.OutputManager.SetOutputAsync(
                        OutputTextBlock,
                        $"Welcome {App.user.UserName}"
                    );
                }
            }
            else
            {
                MessageBox.Show("Error");
            }
        }
        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _ = Utils.OutputManager.SetOutputAsync(
                OutputTextBlock,
                "Loading settings..."
                );
            await Task.Delay(2500);
            this.Content = new InformationsPage();
        }
        private async void SystemCommands_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                await Utils.SystemCommands.ClearSystemCache();
                _ = Utils.OutputManager.SetOutputAsync(
                    OutputTextBlock,
                    "All processes are done!"
                    );
            }
            catch (Exception ex)
            {
                _ = Utils.OutputManager.SetOutputAsync(
                    OutputTextBlock,
                    $"An error occurred: {ex.Message}"
                );
            }
        }

        private void Boostfps_Button(object sender, EventArgs e)
        {

        }

        private async void AccountButton(object sender, RoutedEventArgs e)
        {
            AccountSettingsButton.IsEnabled = true;

            _ = Utils.OutputManager.SetOutputAsync(
                OutputTextBlock,
                "Loading Account settings..."
            );
            AccountSettingsButton.IsEnabled = false;
            await Task.Delay(2500);
            this.Content = new InformationsPage();
        }

        private void CloseAppBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
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
    }
}
