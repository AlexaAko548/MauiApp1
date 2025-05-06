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
            var firstName = FirstNameEntry.Text?.Trim();
            var lastName = LastNameEntry.Text?.Trim();
            var email = EmailEntry.Text?.Trim();
            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;

            if (string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                await DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            var signupData = new
            {
                first_name = firstName,
                last_name = lastName,
                email = email,
                password = password,
                confirm_password = confirmPassword
            };

            var json = System.Text.Json.JsonSerializer.Serialize(signupData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.PostAsync("https://todo-list.dcism.org/signup_action.php", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var result = System.Text.Json.JsonSerializer.Deserialize<SignUpResponse>(responseBody);

                if (result != null && result.status == 200)
                {
                    await DisplayAlert("Success", result.message, "OK");
                    await Shell.Current.GoToAsync("//SignInPage");
                }
                else
                {
                    await DisplayAlert("Error", result?.message ?? "An error occurred.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            }
        }

        // Helper class to handle the response JSON
        public class SignUpResponse
        {
            public int status { get; set; }
            public string message { get; set; }
        }


        // Navigate back to the SignInPage
        private async void GoBackButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignInPage");
        }
    }
}
