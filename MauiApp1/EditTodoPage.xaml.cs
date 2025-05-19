using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using MauiApp1.Models;
using System.Linq;


namespace MauiApp1
{
    // Bind the "item_id" query parameter to this property
    [QueryProperty(nameof(ItemId), "item_id")]
    public partial class EditTodoPage : ContentPage
    {
        private TodoItem selectedTask;
        private int itemId;

        public EditTodoPage()
        {
            InitializeComponent();
        }

        // This property is set by the shell when navigating to this page:
        // e.g. Shell.Current.GoToAsync($"//EditTodoPage?item_id={someId}");
        public string ItemId
        {
            get => itemId.ToString();
            set
            {
                if (int.TryParse(value, out var id))
                {
                    itemId = id;
                    LoadTodoItem(id);
                }
            }
        }

        // Pull the task from your global Session (set when you fetched it originally)
        private void LoadTodoItem(int id)
        {
            selectedTask = Session.CurrentTasks.FirstOrDefault(t => t.item_id == id);
            if (selectedTask != null)
            {
                TitleEntry.Text = selectedTask.item_name;
                DescriptionEntry.Text = selectedTask.item_description;
            }
        }

        // Called when user taps "Update"
        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            if (selectedTask == null)
            {
                await DisplayAlert("Error", "Task not loaded.", "OK");
                return;
            }

            // Build the payload exactly as specified
            var payload = new
            {
                item_name = TitleEntry.Text,
                item_description = DescriptionEntry.Text,
                item_id = selectedTask.item_id
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var http = new HttpClient();
                var response = await http.PutAsync(
                    "https://todo-list.dcism.org/editItem_action.php",
                    content
                );
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TodoApiResponse>(body);

                if (result.status == 200)
                {
                    await DisplayAlert("Success", "Item updated.", "OK");
                    await Shell.Current.GoToAsync("//TodoPage");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }



        private async void OnCompleteClicked(object sender, EventArgs e)
        {
            if (selectedTask == null)
            {
                await DisplayAlert("Error", "Task not loaded.", "OK");
                return;
            }

            // Build the payload to mark as completed
            var payload = new
            {
                status = "inactive",
                item_id = selectedTask.item_id
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var http = new HttpClient();
                var response = await http.PutAsync(
                    "https://todo-list.dcism.org/statusItem_action.php",
                    content
                );
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TodoApiResponse>(body);

                if (result.status == 200)
                {
                    await DisplayAlert("Completed", "Task marked as complete.", "OK");
                    // Return to main list
                    await Shell.Current.GoToAsync("//TodoPage");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Complete failed: {ex.Message}", "OK");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (selectedTask == null)
            {
                await DisplayAlert("Error", "Task not loaded.", "OK");
                return;
            }

            // Ask the user to confirm
            bool confirm = await DisplayAlert(
                "Confirm Delete",
                $"Delete '{selectedTask.Title}'?",
                "Yes", "No"
            );
            if (!confirm) return;

            try
            {
                using var client = new HttpClient();
                // Call the DELETE endpoint with your item_id
                var response = await client.DeleteAsync(
                    $"https://todo-list.dcism.org/deleteItem_action.php?item_id={selectedTask.item_id}"
                );

                // Read and deserialize the JSON response
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TodoApiResponse>(json);

                if (result.status == 200)
                {
                    await DisplayAlert("Deleted", "Task deleted.", "OK");
                    // Go back to the main to‑do list
                    await Shell.Current.GoToAsync("//TodoPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to delete task.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Delete failed: {ex.Message}", "OK");
            }
        }

        // Called when the check icon in the bottom nav is tapped
        private async void OnCompletedClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CompletedTodoPage");
        }


        // Called when the check icon in the bottom nav is tapped
        private async void OnListClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("/TodoPage");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }

    }
}
