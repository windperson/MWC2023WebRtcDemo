using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using SharedCallUi.Data;

using WebRtcWasm.Client.Services.WebRtc;

namespace WebRtcWasm.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddSingleton<WeatherForecastService>();

            builder.Services.AddScoped<IWebRtcService, WebRtcService>();

            await builder.Build().RunAsync();
        }
    }
}