using Android.App;
using Android.Content.PM;
using Android.OS;

using Javax.Net.Ssl;
using Org.Apache.Http.Conn.Ssl;

namespace WebRtcMauiApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
#if DEBUG
            // Bypass CN mismatch by android emulator's 10.0.2.2 IP address
            HttpsURLConnection.DefaultHostnameVerifier = new AllowAllHostnameVerifier();
#endif
            base.OnCreate(savedInstanceState);
        }
    }
}