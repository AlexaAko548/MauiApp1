using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class SignInPage : ContentPage
    {
        public SignInPage()
        {
            InitializeComponent();
        }

        // Handle Sign In Button Click
        private async void SignInButton_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(EmailEntry.Text) && !string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                // Redirect to TodoPage
                await Shell.Current.GoToAsync("//TodoPage");
            }
            else
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
            }
        }

        // Navigate to the SignUpPage
        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignUpPage");
        }
    }
}
