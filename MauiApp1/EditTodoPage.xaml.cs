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


        private async void Sclicked(object sender, EventArgs e)
        {
            // Navigate back to the TodoPage using Shell's GoToAsync method
            await Shell.Current.GoToAsync("//TodoPage");
        }

    }
}
