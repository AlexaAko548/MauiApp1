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
            Tasks = new ObservableCollection<TodoItem>
            {
                new TodoItem { Title = "Watch me Whip 1", IsCompleted = false },
                new TodoItem { Title = "Watch me Nae Nae", IsCompleted = false }
            };

            BindingContext = this;
        }

        private void OnAddTaskClicked(object sender, EventArgs e)
        {
            Tasks.Add(new TodoItem { Title = "New Task", IsCompleted = false });
        }

        // Delete method to remove the task from the ObservableCollection
        private void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                var taskToDelete = button.BindingContext as TodoItem;
                if (taskToDelete != null)
                {
                    Tasks.Remove(taskToDelete);
                }
            }
        }
        private async void OnTodoItemTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement visualElement && visualElement.BindingContext is TodoItem tappedItem)
            {
                // Use the full URI to navigate, including the parameter
                var route = $"///AddTodoPage?SelectedTask={Uri.EscapeDataString(tappedItem.Title)}";
                await Shell.Current.GoToAsync(route);
            }
        }


    }

    public class TodoItem
    {
        public string Title { get; set; }
        public bool IsCompleted { get; set; }

        // DeleteCommand now invokes the OnDeleteTaskClicked method directly
        public ICommand DeleteCommand => new Command(() => App.Current.MainPage.DisplayAlert("Delete", $"Delete {Title}?", "OK"));
    }
}
