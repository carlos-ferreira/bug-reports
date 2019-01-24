using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;

namespace WebviewFocusIssue.Droid.Renderers
{
    public class WebViewBridge : Java.Lang.Object
    {
        readonly WeakReference<IWebViewBridge> _webViewBridge;

        public WebViewBridge(IWebViewBridge webViewBridge)
        {
            _webViewBridge = new WeakReference<IWebViewBridge>(webViewBridge);
        }

        [JavascriptInterface]
        [Export("onInitializeComplete")]
        public void OnInitializeComplete()
        {
            if (_webViewBridge != null && _webViewBridge.TryGetTarget(out IWebViewBridge webViewBridge))
            {
                webViewBridge.OnInitializeComplete();
            }
        }

        [JavascriptInterface]
        [Export("onGetContents")]
        public void OnGetContents(string title, string content, string action)
        {
            if (_webViewBridge != null && _webViewBridge.TryGetTarget(out IWebViewBridge webViewBridge))
            {
                webViewBridge.OnGetContents(title, content, action);
            }
        }

        [JavascriptInterface]
        [Export("onSaveEditorState")]
        public void OnSaveEditorState(string state)
        {

            if (_webViewBridge != null && _webViewBridge.TryGetTarget(out IWebViewBridge webViewBridge))
            {
                webViewBridge.OnSaveEditorState(state);
            }
        }

        [JavascriptInterface]
        [Export("onShowImageDetails")]
        public void OnShowImageDetails(string imageSource,
                                string imageWidth,
                                string imageHeight,
                                string legend,
                                string title,
                                string alt,
                                string alignment)
        {

            if (_webViewBridge != null && _webViewBridge.TryGetTarget(out IWebViewBridge webViewBridge))
            {
                webViewBridge.OnShowImageDetails(imageSource, imageWidth, imageHeight, legend, title, alt, alignment);
            }
        }
    }
}