using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiApp1
{
    public partial class TodoPage : ContentPage
    {
        public ObservableCollection<TodoItem> Tasks { get; set; }

        public TodoPage()
        {
            InitializeComponent();

            // Use the global pending tasks collection
            Tasks = TodoService.PendingTasks;

            // For demonstration, add some initial tasks if the list is empty
            if (Tasks.Count == 0)
            {
                Tasks.Add(new TodoItem { Title = "Watch me Whip 1", IsCompleted = false });
                Tasks.Add(new TodoItem { Title = "Watch me Nae Nae", IsCompleted = false });
            }

            BindingContext = this;
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            // Navigate to AddTodoPage to create a new todo
            await Shell.Current.GoToAsync("//AddTodoPage");
        }

        private void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            if (sender is TapGestureRecognizer tapGesture && tapGesture.CommandParameter is TodoItem taskToDelete)
            {
                // Instead of App.Current.MainPage.DisplayAlert(...)
                // we can just remove the item or show a quick alert:

                // 1) Remove the item
                Tasks.Remove(taskToDelete);

                // 2) Possibly show an alert using `this` or `Shell.Current`:
                this.DisplayAlert("Deleted", $"Deleted {taskToDelete.Title}", "OK");
                // or
                // Shell.Current.DisplayAlert("Deleted", $"Deleted {taskToDelete.Title}", "OK");
            }
        }

        private void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TodoItem checkedItem)
            {
                if (e.Value)
                {
                    // Defer removal/navigation until the next UI cycle
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        TodoService.PendingTasks.Remove(checkedItem);
                        TodoService.CompletedTasks.Add(checkedItem);

                        await Shell.Current.GoToAsync("//CompletedTodoPage");
                    });
                }
            }
        }

        private async void OnTodoItemTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement visualElement && visualElement.BindingContext is TodoItem tappedItem)
            {
                // Navigate to the EditTodoPage
                var route = $"//EditTodoPage?SelectedTask={Uri.EscapeDataString(tappedItem.Title)}";
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

    public class TodoItem
    {
        // Fix #1: Provide a default value to avoid CS8618
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
        public ICommand DeleteCommand => new Command(() =>
        {
            // If you are sure Shell is not null:
            Shell.Current.DisplayAlert("Delete", $"Delete {Title}?", "OK");
        });
    }

}
