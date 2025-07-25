using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;


namespace MLKitBarcodeScannerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
#if ANDROID
    builder.Services.AddSingleton<IBarcodeScanner, Platforms.Android.CameraXBarcodeScannerService>();
#endif

            // Register pages
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}
