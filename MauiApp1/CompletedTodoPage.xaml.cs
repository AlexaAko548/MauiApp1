using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class CompletedTodoPage : ContentPage
    {
        public ObservableCollection<TodoItem> CompletedTasks { get; set; }

        public CompletedTodoPage()
        {
            InitializeComponent();
            CompletedTasks = TodoService.CompletedTasks;
            BindingContext = this;
        }

        // Handle tapping a completed item in the ListView
        private async void OnCompletedTodoItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is TodoItem tappedItem)
            {
                var route = $"//EditTodoPage?SelectedTask={Uri.EscapeDataString(tappedItem.Title)}";
                await Shell.Current.GoToAsync(route);

                // Deselect the tapped item
                ((ListView)sender).SelectedItem = null;
            }
        }

        // Handle deleting a task via delete icon
        private void OnDeleteCompletedTaskClicked(object sender, EventArgs e)
        {
            if (sender is VisualElement element && element.BindingContext is TodoItem taskToDelete)
            {
                CompletedTasks.Remove(taskToDelete);
            }
        }

        private async void OnListClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodoPage");
        }

        private async void OnProfileCLicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}
