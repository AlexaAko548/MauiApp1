using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Maui.Storage;

namespace MauiApp1
{
    public partial class AddTodoPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://todo-list.dcism.org";
        private int userId => Preferences.Get("user_id", -1);

        public AddTodoPage()
        {
            InitializeComponent();
        }

        //bottom navigation
        private async void OnCheckClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CompletedTodoPage");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodoPage");
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (userId == -1)
            {
                await DisplayAlert("Error", "User not signed in. Please log in again.", "OK");
                await Shell.Current.GoToAsync("//SignInPage");
                return;
            }

            string title = TitleEntry.Text;
            string description = DescriptionEntry.Text;



            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                await DisplayAlert("Missing Info", "Please enter a title and description.", "OK");
                return;
            }

            var todoItem = new
            {
                item_name = title,
                item_description = description,
                user_id = userId
            };

            string jsonData = JsonConvert.SerializeObject(todoItem);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{BaseUrl}/addItem_action.php", content);
                string result = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeObject<dynamic>(result);

                if ((int)responseData.status == 200)
                {
                    await DisplayAlert("Success", "Task added successfully!", "OK");
                    await Shell.Current.GoToAsync("//TodoPage");
                }
                else
                {
                    await DisplayAlert("Error", (string)responseData.message, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
            }

            TitleEntry.Text = string.Empty;
            DescriptionEntry.Text = string.Empty;
        }
    }
}