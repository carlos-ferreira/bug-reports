using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using WebviewFocusIssue.Controls;
using Xamarin.Forms;

namespace WebviewFocusIssue.Droid.Renderers
{
    public class CustomWebViewClient : WebViewClient
    {
        WebNavigationResult _navigationResult = WebNavigationResult.Success;
        HybridWebViewRenderer _renderer;

        public CustomWebViewClient(HybridWebViewRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");
            _renderer = renderer;
        }

        protected CustomWebViewClient(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public override void OnPageStarted(global::Android.Webkit.WebView view, string url, Bitmap favicon)
        {
            if (_renderer?.Element == null || url == HybridWebViewRenderer.AssetBaseUrl)
                return;

            var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url }, url);

            _renderer.ElementController.SendNavigating(args);
            _navigationResult = WebNavigationResult.Success;

            _renderer.UpdateCanGoBackForward();

            if (args.Cancel)
            {
                _renderer.Control.StopLoading();
            }
            else
            {
                base.OnPageStarted(view, url, favicon);
            }
        }

        public override void OnPageFinished(global::Android.Webkit.WebView view, string url)
        {
            if (_renderer?.Element == null || url == HybridWebViewRenderer.AssetBaseUrl)
                return;

            var source = new UrlWebViewSource { Url = url };
            _renderer.IgnoreSourceChanges = true;
            _renderer.ElementController.SetValueFromRenderer(HybridWebView.SourceProperty, source);
            _renderer.IgnoreSourceChanges = false;

            var args = new WebNavigatedEventArgs(WebNavigationEvent.NewPage, source, url, _navigationResult);

            _renderer.ElementController.SendNavigated(args);

            _renderer.UpdateCanGoBackForward();

            base.OnPageFinished(view, url);
        }

        [Obsolete("OnReceivedError is obsolete as of version 2.3.0. This method was deprecated in API level 23.")]
        public override void OnReceivedError(global::Android.Webkit.WebView view, ClientError errorCode, string description, string failingUrl)
        {
            _navigationResult = WebNavigationResult.Failure;
            if (errorCode == ClientError.Timeout)
                _navigationResult = WebNavigationResult.Timeout;
#pragma warning disable 618
            base.OnReceivedError(view, errorCode, description, failingUrl);
#pragma warning restore 618
        }

        public override void OnReceivedError(global::Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
        {
            _navigationResult = WebNavigationResult.Failure;
            if (error.ErrorCode == ClientError.Timeout)
                _navigationResult = WebNavigationResult.Timeout;
            base.OnReceivedError(view, request, error);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                _renderer = null;
        }
    }
}
