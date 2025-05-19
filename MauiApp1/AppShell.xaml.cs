using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        bool _hasRestored = false;

        public AppShell()
        {
            InitializeComponent();

            // Register all your routes here
            Routing.RegisterRoute(nameof(AddTodoPage), typeof(AddTodoPage));
            Routing.RegisterRoute(nameof(EditTodoPage), typeof(EditTodoPage));
            Routing.RegisterRoute(nameof(CompletedTodoPage), typeof(CompletedTodoPage));
            Routing.RegisterRoute(nameof(EditCompletedTodoPage), typeof(EditCompletedTodoPage));
            Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));

            // Hook into the shell’s Appearing event
            this.Appearing += OnShellAppearing;
        }

        async void OnShellAppearing(object sender, EventArgs e)
        {
            // Only do this restoration once
            if (_hasRestored)
                return;
            _hasRestored = true;

            // ─── DEBUG: always clear saved user_id & last_route on debug builds
            #if DEBUG
                        Preferences.Remove("user_id");
                        Preferences.Remove("last_route");
            #endif

            // Determine “signed in” by the presence of a valid user_id
            var userId = Preferences.Get("user_id", -1);
            if (userId >= 0)
            {
                // restore the last route (or default to TodoPage)
                var last = Preferences.Get("last_route", "//TodoPage");
                await this.GoToAsync(last);
            }
            else
            {
                // ensure we land on the SignInPage and clear any history
                await this.GoToAsync("//SignInPage");
            }
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);

            // Save every successful navigation so we can restore it later
            var userId = Preferences.Get("user_id", -1);
            if (userId >= 0)
            {
                Preferences.Set("last_route", args.Current.Location.ToString());
            }
        }
    }
}
