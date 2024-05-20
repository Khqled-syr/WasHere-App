using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WasHere.Database;
using WasHere.Utils;

namespace WasHere.ViewModel
{
    public partial class UsersEditPage : Page
    {
        public UsersEditPage()
        {
            InitializeComponent();
            LoadUsers();
        }


        private async void LoadUsers()
        {
            using (var dbContext = new DatabaseContext())
            {
                var users = await dbContext.Users.ToListAsync();
                UsersComboBox.ItemsSource = users;
                UsersComboBox.DisplayMemberPath = "UserName";
                UsersComboBox.SelectedValuePath = "Id";
            }
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainUI mainUI = new MainUI();
            mainUI.Visibility = Visibility.Visible;
            await Task.Delay(10);
            System.Windows.Window win = (System.Windows.Window)Parent;
            win.Close();

        }

        private async void BanUser_Button(object sender, RoutedEventArgs e)
        {
            if (UsersComboBox.SelectedItem is User selectedUser)
            {
                // Prompt for confirmation
                var result = MessageBox.Show($"Are you sure you want to ban the user '{selectedUser.UserName}'?",
                                             "Confirm Ban",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                // If the user confirms, proceed with the ban
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (Utils.PermsChecker.IsUserAdmin(selectedUser.UserName))
                        {
                            MessageBox.Show($"You can't ban this user!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Ban the user
                        using (var dbContext = new DatabaseContext())
                        {
                            KeyAuthApi.KeyAuthApp.ban(selectedUser.UserName , "Breaking the rules!");
                            //dbContext.Users.Remove(selectedUser);
                            //await dbContext.SaveChangesAsync();
                        }
                        // Refresh the users list after banning
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that occur during the ban process
                        MessageBox.Show($"An error occurred while banning the user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user to ban.");
            }
        }
    }
}
