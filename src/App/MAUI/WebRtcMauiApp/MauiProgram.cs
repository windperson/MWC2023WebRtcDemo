using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Logging;

using SharedCallUi.Data;

using WebRtcMauiApp.Services.WebRtc;

namespace WebRtcMauiApp
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
                });

#if ANDROID
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(BlazorWebView), typeof(Platforms.Android.Handlers.MauiBlazorAndroidWebViewHandler));
            });
#endif

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddScoped<IWebRtcService, WebRtcService>();

#if ANDROID && DEBUG
            Platforms.Android.DangerousAndroidMessageHandlerEmitter.Register();
            Platforms.Android.DangerousTrustProvider.Register();
#endif

            return builder.Build();
        }
    }
}