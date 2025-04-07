using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            // Navigate to TodoPage after sign up
            await Shell.Current.GoToAsync("//TodoPage");
        }

        // Navigate back to the SignInPage
        private async void GoBackButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignInPage");
        }
    }
}
