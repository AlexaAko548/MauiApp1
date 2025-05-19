using System.Linq;
using System.Net.Http;
using System.Text;
using MauiApp1.Models;
using Newtonsoft.Json;

namespace MauiApp1
{
    [QueryProperty(nameof(ItemId), "item_id")]
    public partial class EditCompletedTodoPage : ContentPage
    {

        void ShowLoading() => LoadingOverlay.IsVisible = true;
        void HideLoading() => LoadingOverlay.IsVisible = false;

        private TodoItem? _todoItem;
        private int _itemId;

        public EditCompletedTodoPage()
        {
            InitializeComponent();
        }

        public string ItemId
        {
            get => _itemId.ToString();
            set
            {
                if (int.TryParse(value, out var id))
                {
                    _itemId = id;
                    LoadTodoItem(id);
                }
            }
        }

        void LoadTodoItem(int id)
        {
            _todoItem = Session.CurrentTasks.FirstOrDefault(t => t.item_id == id);
            if (_todoItem == null)
                return;

            // use null‐propagation so compiler knows you’ve handled possible null
            TitleEntry.Text = _todoItem.item_name;
            DescriptionEntry.Text = _todoItem.item_description;
        }

        async void OnUpdateClicked(object sender, EventArgs e)
        {
            if (_todoItem == null) return;

            ShowLoading();
            try
            {
                var payload = new
                {
                    item_id = _todoItem.item_id,
                    item_name = TitleEntry?.Text,
                    item_description = DescriptionEntry?.Text
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var http = new HttpClient();
                var resp = await http.PutAsync(
                    "https://todo-list.dcism.org/editItem_action.php",
                    content
                );

                // guard against null
                var body = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TodoApiResponse>(body);
                if (result?.status == 200)
                {
                    await DisplayAlert("Success", "Task updated.", "OK");
                    await Shell.Current.GoToAsync("//CompletedTodoPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update task.", "OK");
                }
            }
            finally
            {
                HideLoading();
            }
            
        }

        async void OnIncompleteClicked(object sender, EventArgs e)
        {
            if (_todoItem == null) return;

            ShowLoading();
            try
            {
                var data = new { status = "active", item_id = _todoItem.item_id };
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var http = new HttpClient();
                var resp = await http.PutAsync(
                    "https://todo-list.dcism.org/statusItem_action.php",
                    content
                );

                if (resp.IsSuccessStatusCode)
                {
                    await DisplayAlert("Moved", "Task moved to active list.", "OK");
                    await Shell.Current.GoToAsync("//TodoPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to move task.", "OK");
                }
            }
            finally
            {
                HideLoading();
            }

        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_todoItem == null) return;

            bool confirm = await DisplayAlert(
                "Confirm",
                $"Delete '{_todoItem.item_name}'?",
                "Yes", "No"
            );
            if (!confirm) return;

            ShowLoading();
            try
            {
                using var http = new HttpClient();
                var resp = await http.DeleteAsync(
                    $"https://todo-list.dcism.org/deleteItem_action.php?item_id={_todoItem.item_id}"
                );

                if (resp.IsSuccessStatusCode)
                {
                    await DisplayAlert("Deleted", "Task deleted.", "OK");
                    await Shell.Current.GoToAsync("//CompletedTodoPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to delete task.", "OK");
                }
            }
            finally
            {
                HideLoading();
            }
          
        }

        async void OnListClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//TodoPage");
        async void OnCompletedClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//CompletedTodoPage");
        async void OnProfileClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//ProfilePage");
    }
}
