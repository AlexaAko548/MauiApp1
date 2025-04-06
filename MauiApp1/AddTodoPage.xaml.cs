using System.Windows.Input;

namespace MauiApp1
{
    public partial class AddTodoPage : ContentPage
    {
        public AddTodoPage()
        {
            InitializeComponent();
            BindingContext = this;
        }


        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Navigate back to the TodoPage using Shell's GoToAsync method
            await Shell.Current.GoToAsync("//TodoPage");
        }

    }
}
