using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.Webkit;

using Microsoft.AspNetCore.Components.WebView.Maui;

namespace WebRtcMauiApp.Platforms.Android.Handlers
{
    internal class AllPermissionPassWebChromeClient : WebChromeClient
    {
        public override void OnPermissionRequest(PermissionRequest request)
        {
            // TODO: We should probably check the request for the permission before granting it
            try
            {
                request.Grant(request.GetResources());
                base.OnPermissionRequest(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class MauiBlazorAndroidWebViewHandler : BlazorWebViewHandler
    {
        protected override global::Android.Webkit.WebView CreatePlatformView()
        {
            var view = base.CreatePlatformView();

            view.SetWebChromeClient(new AllPermissionPassWebChromeClient());

            return view;
        }
    }
}
