using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;
using WebKit;

namespace WebviewFocusIssue.iOS.Controls
{
    public class CustomWebView : WKWebView
    {
        public CustomWebView(NSCoder coder) : base(coder)
        {
        }

        public CustomWebView(CGRect frame, WKWebViewConfiguration configuration) : base(frame, configuration)
        {
        }

        protected CustomWebView(NSObjectFlag t) : base(t)
        {
        }

        protected internal CustomWebView(IntPtr handle) : base(handle)
        {
        }

       

    }
}