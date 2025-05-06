using System.Windows.Input;

namespace MauiApp1
{
    public partial class AddTodoPage : ContentPage
    {
        // 1. Declare the property
        public ICommand NavigateBackCommand { get; private set; }

        public AddTodoPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodoPage");
        }
    }
}
