using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using WasHere.Database;
using Windows.System;

namespace WasHere.ViewModel
{
    public partial class LoginUI : Window
    {

        public LoginUI()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserTextBox.Text;
            string password = PasswordBox.Password;

            using (var dbContext = new DatabaseContext())
            {
                Database.User? user = dbContext.Users.FirstOrDefault(user => user.UserName.ToLower() == username.ToLower());

                if (user != null && user.Password == password)
                {
                    // Authentication successful
                    App.user = user;
                    MainUI mainUI = new MainUI();
                    mainUI.Show();

                    await Task.Delay(10);
                    this.Close();
                }
                else
                {
                    // Incorrect password
                    MessageBox.Show("Password or Username is not correct, please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    PasswordBox.Clear();
                }
            }
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new RegistrationPage();
        }



        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            // Minimize the window
            this.WindowState = WindowState.Minimized;
        }

        private void CloseAppBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
