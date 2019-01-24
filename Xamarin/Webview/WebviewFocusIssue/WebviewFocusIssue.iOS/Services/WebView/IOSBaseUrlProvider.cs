using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using WebviewFocusIssue.Services.Webview;

namespace WebviewFocusIssue.iOS.Services.WebView
{
    public class IOSBaseUrlProvider : IBaseUrlProvider
    {
        public string GetBaseUrl()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}