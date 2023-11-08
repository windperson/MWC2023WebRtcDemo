namespace WebRtcMauiApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, EventArgs e)
        {
#if DEBUG && MACCATALYST
            if (blazorWebView.Handler.PlatformView is WebKit.WKWebView view)
            {
                view.SetValueForKey(Foundation.NSObject.FromObject(true), new Foundation.NSString("inspectable"));
            }
#endif
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var cameraPermission = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (cameraPermission != PermissionStatus.Granted)
            {
                cameraPermission = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (cameraPermission == PermissionStatus.Granted)
            {
                return;
            }

            await DisplayAlert("Camera permission", "Camera permission is required to use this app", "OK");
        }
    }
}