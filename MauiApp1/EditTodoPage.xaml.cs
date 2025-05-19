using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using MauiApp1.Models;
using System.Linq;

namespace MauiApp1
{
    [QueryProperty(nameof(ItemId), "item_id")]
    public partial class EditTodoPage : ContentPage
    {
        private TodoItem? selectedTask;
        private int itemId;

        public EditTodoPage()
        {
            InitializeComponent();
        }

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

        void LoadTodoItem(int id)
        {
            selectedTask = Session.CurrentTasks.FirstOrDefault(t => t.item_id == id);
            if (selectedTask == null)
                return;

            TitleEntry.Text = selectedTask.item_name;
            DescriptionEntry.Text = selectedTask.item_description;
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            if (selectedTask == null)
            {
                await DisplayAlert("Error", "Task not loaded.", "OK");
                return;
            }

            var payload = new
            {
                item_name = TitleEntry?.Text,
                item_description = DescriptionEntry?.Text,
                item_id = selectedTask.item_id
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            ShowLoading();
            try
            {
                using var http = new HttpClient();
                var response = await http.PutAsync(
                    "https://todo-list.dcism.org/editItem_action.php",
                    content
                );
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TodoApiResponse>(body);

                if (result?.status == 200)
                {
                    await DisplayAlert("Success", "Item updated.", "OK");
                    await Shell.Current.GoToAsync("//TodoPage");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                HideLoading();
            }
        }

        void ShowLoading() => LoadingOverlay.IsVisible = true;
        void HideLoading() => LoadingOverlay.IsVisible = false;

        private async void OnCompleteClicked(object sender, EventArgs e)
        {
            if (selectedTask == null)
            {
                await DisplayAlert("Error", "Task not loaded.", "OK");
                return;
            }

            var payload = new
            {
                status = "inactive",
                item_id = selectedTask.item_id
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            ShowLoading();
            try
            {
                using var http = new HttpClient();
                var response = await http.PutAsync(
                    "https://todo-list.dcism.org/statusItem_action.php",
                    content
                );
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Completed", "Task marked as complete.", "OK");
                    await Shell.Current.GoToAsync("//TodoPage");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Complete failed: {ex.Message}", "OK");
            }
            finally
            {
                HideLoading();
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (selectedTask == null)
            {
                await DisplayAlert("Error", "Task not loaded.", "OK");
                return;
            }

            bool confirm = await DisplayAlert(
                "Confirm Delete",
                $"Delete '{selectedTask.item_name}'?",
                "Yes", "No"
            );
            if (!confirm) return;

            ShowLoading();
            try
            {
                using var client = new HttpClient();
                var response = await client.DeleteAsync(
                    $"https://todo-list.dcism.org/deleteItem_action.php?item_id={selectedTask.item_id}"
                );

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Deleted", "Task deleted.", "OK");
                    await Shell.Current.GoToAsync("//TodoPage");
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

        private async void OnCompletedClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//CompletedTodoPage");

        private async void OnListClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//TodoPage");

        private async void OnProfileClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//ProfilePage");
    }
}
