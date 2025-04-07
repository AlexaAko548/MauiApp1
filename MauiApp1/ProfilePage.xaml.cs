using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private async void OnListClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodoPage");
        }

        private async void OnCheckClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CompletedTodoPage");
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            bool confirmed = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
            if (confirmed)
            {
                // Add sign-out logic here (clear session, navigate to login, etc.)
                await Shell.Current.GoToAsync("//SignInPage"); // Example redirect
            }
        }
    }
}