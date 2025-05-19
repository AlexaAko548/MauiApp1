using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MauiApp1.Models;


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
            bool confirmed = await DisplayAlert(
                "Sign Out",
                "Are you sure you want to sign out?",
                "Yes", "Cancel");

            if (!confirmed)
                return;

            // 1) Clear persisted credentials
            Preferences.Remove("user_id");
            Preferences.Remove("last_route");

            // 2) Clear any in-memory session
            Session.UserId = -1;
            Session.CurrentTasks = new List<TodoItem>();

            // 3) Pop to root and reset the navigation stack
            await Shell.Current.GoToAsync("//SignInPage");

            // (Optional) If you want to remove all pages from the backstack so
            // hardware back really exits from the SignInPage:
            await Shell.Current.Navigation.PopToRootAsync(animated: false);
        }
    }
}