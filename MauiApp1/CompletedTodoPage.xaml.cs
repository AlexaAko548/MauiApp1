using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using MauiApp1.Models;
using Newtonsoft.Json;

namespace MauiApp1
{
    public partial class CompletedTodoPage : ContentPage
    {
        // 1) Initialize inline so CompletedTasks is never null
        public ObservableCollection<TodoItem> CompletedTasks { get; set; } = new();

        public CompletedTodoPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = LoadCompletedTasksAsync();
        }

        private async Task LoadCompletedTasksAsync()
        {
            try
            {
                ShowLoading();
                var url = $"https://todo-list.dcism.org/getItems_action.php?status=inactive&user_id={Session.UserId}";
                using var http = new HttpClient();
                var jsonResponse = await http.GetStringAsync(url);

                // 2) Check for null before you dereference
                var result = JsonConvert.DeserializeObject<TodoApiResponse>(jsonResponse);
                if (result is not null && result.status == 200 && result.data is not null)
                {
                    CompletedTasks.Clear();
                    foreach (var item in result.data.Values)
                        CompletedTasks.Add(item);
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load completed tasks.", "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error fetching completed tasks: {ex.Message}", "OK");
                return;
            }
            finally
            {
                HideLoading();
            }

            // keep session in sync
            Session.CurrentTasks = CompletedTasks.ToList();
        }

        void ShowLoading() => LoadingOverlay.IsVisible = true;
        void HideLoading() => LoadingOverlay.IsVisible = false;

        private async void OnCheckBoxUnchecked(object sender, CheckedChangedEventArgs e)
        {
            // no possible null here: CompletedTasks is always non-null, handler only fires if we have a binding context
            if (sender is CheckBox cb
                && cb.BindingContext is TodoItem item
                && !e.Value)
            {
                var payload = new { status = "active", item_id = item.item_id };
                var body = JsonConvert.SerializeObject(payload);
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                try
                {
                    using var http = new HttpClient();
                    var respJson = await (await http.PutAsync(
                        "https://todo-list.dcism.org/statusItem_action.php",
                        content)).Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<TodoApiResponse>(respJson);
                    if (result is not null && result.status == 200)
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

        private async void OnDeleteCompletedTaskClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn
                && btn.CommandParameter is TodoItem itemToDelete)
            {
                // 3) Use item_name (which is non-null) in the prompt
                bool confirm = await DisplayAlert(
                    "Delete",
                    $"Delete '{itemToDelete.item_name}'?",
                    "Yes", "No"
                );
                if (!confirm)
                    return;

                try
                {
                    using var http = new HttpClient();
                    var resp = await http.DeleteAsync(
                        $"https://todo-list.dcism.org/deleteItem_action.php?item_id={itemToDelete.item_id}"
                    );
                    var respJson = await resp.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TodoApiResponse>(respJson);

                    if (result is not null && result.status == 200)
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
            if (sender is VisualElement ve
                && ve.BindingContext is TodoItem tappedItem)
            {
                await Shell.Current.GoToAsync($"EditCompletedTodoPage?item_id={tappedItem.item_id}");
            }
        }

        private async void OnListClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//TodoPage");

        private async void OnProfileCLicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//ProfilePage");
    }
}
