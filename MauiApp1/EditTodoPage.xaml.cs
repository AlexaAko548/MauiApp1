using System.Windows.Input;

namespace MauiApp1
{
    public partial class EditTodoPage : ContentPage
    {
        public EditTodoPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        // Update button handler
        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            // TODO: Implement updating logic
            // e.g. update the TitleEntry, DescriptionEntry in your data model

            // Navigate back to TodoPage
            await Shell.Current.GoToAsync("//TodoPage");
        }

        // Complete button handler
        private async void OnCompleteClicked(object sender, EventArgs e)
        {
            // TODO: Mark the current item as completed in your data
            // For example:
            //   TodoService.PendingTasks.Remove(selectedItem);
            //   TodoService.CompletedTasks.Add(selectedItem);

            // Navigate to CompletedTodoPage
            await Shell.Current.GoToAsync("//CompletedTodoPage");
        }

        // Delete button handler
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            // TODO: Remove the current item from your data
            // e.g. TodoService.PendingTasks.Remove(selectedItem);

            // Navigate back to TodoPage or show some confirmation
            await Shell.Current.GoToAsync("//TodoPage");
        }

        private async void OnListClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodoPage");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}
