using System.Collections.ObjectModel;
using System.Windows.Input;
using Newtonsoft.Json;

namespace MauiApp1
{
    public partial class TodoPage : ContentPage
    {
        public ObservableCollection<TodoItem> Tasks { get; set; }

        public TodoPage()
        {
            InitializeComponent();

            Tasks = new ObservableCollection<TodoItem>();
            BindingContext = this;

            LoadTasksAsync();
        }


        private async void LoadTasksAsync()
        {
            try
            {
                string url = $"https://todo-list.dcism.org/getItems_action.php?status=active&user_id={Session.UserId}";
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);

                var result = JsonConvert.DeserializeObject<TodoApiResponse>(response);

                if (result.status == 200 && result.data != null)
                {
                    Tasks.Clear();
                    foreach (var item in result.data.Values)
                    {
                        Tasks.Add(item);
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load tasks.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error fetching tasks: {ex.Message}", "OK");
            }
        }


        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            // Navigate to AddTodoPage to create a new todo
            await Shell.Current.GoToAsync("//AddTodoPage");
        }

        private async void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton imageButton && imageButton.CommandParameter is TodoItem taskToDelete)
            {
                // Optional: confirm deletion
                bool confirm = await DisplayAlert("Confirm", $"Delete '{taskToDelete.Title}'?", "Yes", "No");
                if (!confirm) return;

                try
                {
                    using var client = new HttpClient();
                    var response = await client.DeleteAsync($"https://todo-list.dcism.org/deleteItem_action.php?item_id={taskToDelete.item_id}");
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TodoApiResponse>(json);

                    if (result.status == 200)
                    {
                        Tasks.Remove(taskToDelete);
                        await DisplayAlert("Deleted", "Task deleted.", "OK");
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
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTasksAsync(); // Refresh task list every time this page appears
        }


        private async void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TodoItem checkedItem)
            {
                if (e.Value)
                {
                    // Update status on server
                    var url = "https://todo-list.dcism.org/statusItem_action.php";
                    var data = new
                    {
                        status = "inactive",
                        item_id = checkedItem.item_id
                    };

                    var json = JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    using var httpClient = new HttpClient();
                    var response = await httpClient.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Remove the item from the active list
                        Tasks.Remove(checkedItem);

                        // Optional: Navigate or just show confirmation
                        await DisplayAlert("Done", $"{checkedItem.Title} marked as done.", "OK");
                        // await Shell.Current.GoToAsync("//CompletedTodoPage");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to update task status.", "OK");
                        checkBox.IsChecked = false; // Reset checkbox
                    }
                }
            }
        }


        private async void OnTodoItemTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement visualElement && visualElement.BindingContext is TodoItem tappedItem)
            {
                // Navigate to the EditTodoPage
                var route = $"//EditTodoPage?item_id={tappedItem.item_id}";
                await Shell.Current.GoToAsync(route);

            }
        }

        // Navigate to the CompletedTodoPage when the check icon is tapped
        private async void OnCheckClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CompletedTodoPage");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }


    public class TodoApiResponse
    {
        public int status { get; set; }
        public Dictionary<string, TodoItem> data { get; set; }
        public string count { get; set; }
    }




    public class TodoItem
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_description { get; set; }
        public string status { get; set; }
        public int user_id { get; set; }
        public string timemodified { get; set; }

        // Helper for UI
        public string Title => item_name;
        public bool IsCompleted => status == "inactive";
    }


}