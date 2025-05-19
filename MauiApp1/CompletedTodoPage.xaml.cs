using System.Collections.ObjectModel;
using Newtonsoft.Json;
using MauiApp1.Models;
using System.Linq;        // for ToList() and FirstOrDefault()


namespace MauiApp1
{
    public partial class CompletedTodoPage : ContentPage
    {
        public ObservableCollection<TodoItem> CompletedTasks { get; set; }

        public CompletedTodoPage()
        {
            InitializeComponent();
            CompletedTasks = new ObservableCollection<TodoItem>();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCompletedTasksAsync(); // Refresh on page appear
        }

        private async void LoadCompletedTasksAsync()
        {
            try
            {
                string url = $"https://todo-list.dcism.org/getItems_action.php?status=inactive&user_id={Session.UserId}";
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);

                var result = JsonConvert.DeserializeObject<TodoApiResponse>(response);

                if (result.status == 200 && result.data != null)
                {
                    CompletedTasks.Clear();
                    foreach (var item in result.data.Values)
                    {
                        CompletedTasks.Add(item);
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load completed tasks.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error fetching completed tasks: {ex.Message}", "OK");
            }

            Session.CurrentTasks = CompletedTasks.ToList();
        }

        private async void OnCheckBoxUnchecked(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TodoItem item)
            {
                if (!e.Value)
                {
                    var data = new { status = "active", item_id = item.item_id };
                    var content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

                    try
                    {
                        using var client = new HttpClient();
                        var response = await client.PutAsync("https://todo-list.dcism.org/statusItem_action.php", content);
                        var resultJson = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TodoApiResponse>(resultJson);

                        if (result.status == 200)
                        {
                            CompletedTasks.Remove(item);
                            await DisplayAlert("Restored", "Task has been moved back to active.", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Failed to restore task.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
                    }
                }
            }
        }

        private async void OnDeleteCompletedTaskClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is TodoItem itemToDelete)
            {
                bool confirm = await DisplayAlert("Delete", $"Delete '{itemToDelete.Title}'?", "Yes", "No");
                if (!confirm) return;

                try
                {
                    var url = $"https://todo-list.dcism.org/deleteItem_action.php?item_id={itemToDelete.item_id}";
                    using var client = new HttpClient();
                    var response = await client.DeleteAsync(url);
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TodoApiResponse>(json);

                    if (result.status == 200)
                    {
                        CompletedTasks.Remove(itemToDelete);
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

        private async void OnCompletedTodoItemTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement visualElement && visualElement.BindingContext is TodoItem tappedItem)
            {
                var route = $"//EditTodoPage?SelectedTask={Uri.EscapeDataString(tappedItem.Title)}";
                await Shell.Current.GoToAsync(route);
            }
        }

        private async void OnListClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//TodoPage");

        private async void OnProfileCLicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//ProfilePage");
    }
}