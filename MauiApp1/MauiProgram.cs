using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if ANDROID
    EntryHandler.Mapper.AppendToMapping("CustomPlaceholder", (handler, view) =>
    {
        handler.PlatformView.SetHintTextColor(Android.Graphics.Color.Black);
    });

    EditorHandler.Mapper.AppendToMapping("CustomPlaceholder", (handler, view) =>
    {
        handler.PlatformView.SetHintTextColor(Android.Graphics.Color.Black);
    });

#endif


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
