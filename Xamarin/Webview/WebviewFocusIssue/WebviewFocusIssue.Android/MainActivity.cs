using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using WebviewFocusIssue.Services.Webview;
using WebviewFocusIssue.Droid.Services.Webview;

namespace WebviewFocusIssue.Droid
{
    [Activity(Label = "WebviewFocusIssue", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new AndroidInitializer()));
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {


        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IBaseUrlProvider, AndroidBaseUrlProvider>();
        }
    }
}