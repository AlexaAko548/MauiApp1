using System.Collections.ObjectModel;
using System.Windows.Input;
using Newtonsoft.Json;
using MauiApp1.Models;
using System.Linq;               // for .ToList() and .FirstOrDefault()


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
                ShowLoading();
                string url = $"https://todo-list.dcism.org/getItems_action.php?status=active&user_id={Session.UserId}";
                using var httpClient = new HttpClient();

                var json = await httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<TodoApiResponse>(json);


                


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
            finally
            {
                HideLoading();
            }

            Session.CurrentTasks = Tasks.ToList();
        }

        // call this before any await-based work
        void ShowLoading() => LoadingOverlay.IsVisible = true;
        void HideLoading() => LoadingOverlay.IsVisible = false;


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

                ShowLoading();
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
                finally
                {
                    HideLoading();
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

            ShowLoading();
            try
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
            finally
            {
                HideLoading();
            }
          
        }


        private async void OnTodoItemTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement visualElement && visualElement.BindingContext is TodoItem tappedItem)
            {
                await Shell.Current.GoToAsync($"EditTodoPage?item_id={tappedItem.item_id}");

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



}