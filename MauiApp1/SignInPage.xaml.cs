using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using static MauiApp1.SignUpPage;

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
            var email = EmailEntry.Text?.Trim();
            var password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            string url = $"https://todo-list.dcism.org/signin_action.php?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";

            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                var result = System.Text.Json.JsonSerializer.Deserialize<SignInResponse>(responseBody);

                if (result != null && result.status == 200)
                {
                    await DisplayAlert("Success", result.message, "OK");

                    // (Optional) Store user info if you need it later
                    var user = result.data;
                    Preferences.Set("user_id", user.id); 
                    Console.WriteLine($"Welcome, {user.fname} {user.lname} (ID: {user.id})");

                    Session.UserId = result.data.id; // Set after login


                    // Navigate to TodoPage
                    await Shell.Current.GoToAsync("//TodoPage");
                }
                else
                {
                    await DisplayAlert("Error", result?.message ?? "Login failed.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            }
        }

        public class SignInResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public UserData data { get; set; }
        }

        public class UserData
        {
            public int id { get; set; }
            public string fname { get; set; }
            public string lname { get; set; }
            public string email { get; set; }
            public string timemodified { get; set; }
        }




        // Navigate to the SignUpPage
        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignUpPage");
        }
    }
}
