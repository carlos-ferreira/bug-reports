using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WebviewFocusIssue.Services.Webview;

namespace WebviewFocusIssue.Droid.Services.Webview
{
    public class AndroidBaseUrlProvider : IBaseUrlProvider
    {
        public string GetBaseUrl()
        {
            return "file:///android_asset";
        }
    }
}