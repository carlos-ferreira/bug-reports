using System.Threading.Tasks;
using Android.Content;
using Android.Webkit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using MixedContentHandling = Android.Webkit.MixedContentHandling;
using AWebView = Android.Webkit.WebView;
using System;
using Android.App;
using Android.OS;
using System.ComponentModel;
using WebviewFocusIssue.Controls;
using WebviewFocusIssue.Droid.Renderers;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace WebviewFocusIssue.Droid.Renderers
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, AWebView>, IWebViewDelegate, IWebViewBridge
    {
        public const string AssetBaseUrl = "file:///android_asset/";

        WebViewClient _webViewClient;
        CustomWebChromeClient _webChromeClient;

        protected internal IWebViewController ElementController => Element;
        protected internal bool IgnoreSourceChanges { get; set; }

        public HybridWebViewRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }

        [Obsolete("This constructor is obsolete as of version 2.5. Please use WebViewRenderer(Context) instead.")]
        public HybridWebViewRenderer()
        {
            AutoPackage = false;
        }

        public void LoadHtml(string html, string baseUrl)
        {
            Control.LoadDataWithBaseURL(baseUrl ?? AssetBaseUrl, html, "text/html", "UTF-8", null);
        }

        public void LoadUrl(string url)
        {
            Control.LoadUrl(url);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Element != null)
                {
                    Control?.StopLoading();

                    ElementController.EvalRequested -= OnEvalRequested;
                    ElementController.GoBackRequested -= OnGoBackRequested;
                    ElementController.GoForwardRequested -= OnGoForwardRequested;

                    _webViewClient?.Dispose();
                    _webChromeClient?.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected virtual WebViewClient GetWebViewClient()
        {
            return new CustomWebViewClient(this);
        }

        protected virtual CustomWebChromeClient GetFormsWebChromeClient()
        {
            return new CustomWebChromeClient();
        }

        protected override Size MinimumSize()
        {
            return new Size(Context.ToPixels(40), Context.ToPixels(40));
        }

        protected override AWebView CreateNativeControl()
        {
            return new AWebView(Context);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var webView = CreateNativeControl();
#pragma warning disable 618 // This can probably be replaced with LinearLayout(LayoutParams.MatchParent, LayoutParams.MatchParent); just need to test that theory
                webView.LayoutParameters = new global::Android.Widget.AbsoluteLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent, 0, 0);
#pragma warning restore 618

                _webViewClient = GetWebViewClient();
                webView.SetWebViewClient(_webViewClient);

                _webChromeClient = GetFormsWebChromeClient();
                _webChromeClient.SetContext(Context as Activity);
                webView.SetWebChromeClient(_webChromeClient);
                webView.AddJavascriptInterface(new WebViewBridge(this), "BloggerPro");
                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.DomStorageEnabled = false;
                SetNativeControl(webView);
            }

            if (e.OldElement != null)
            {
                var oldElementController = e.OldElement as IWebViewController;
                Control.RemoveJavascriptInterface("BloggerPro");
                oldElementController.EvalRequested -= OnEvalRequested;
                oldElementController.EvaluateJavaScriptRequested -= OnEvaluateJavaScriptRequested;
                oldElementController.GoBackRequested -= OnGoBackRequested;
                oldElementController.GoForwardRequested -= OnGoForwardRequested;
            }

            if (e.NewElement != null)
            {
                var newElementController = e.NewElement as IWebViewController;
                newElementController.EvalRequested += OnEvalRequested;
                newElementController.EvaluateJavaScriptRequested += OnEvaluateJavaScriptRequested;
                newElementController.GoBackRequested += OnGoBackRequested;
                newElementController.GoForwardRequested += OnGoForwardRequested;

                UpdateMixedContentMode();
            }

            Load();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case "Source":
                    Load();
                    break;
                case "MixedContentMode":
                    UpdateMixedContentMode();
                    break;
            }
        }

        void Load()
        {
            if (IgnoreSourceChanges)
                return;

            Element.Source?.Load(this);

            UpdateCanGoBackForward();
        }

        void OnEvalRequested(object sender, EvalRequested eventArg)
        {
            LoadUrl("javascript:" + eventArg.Script);
        }

        async Task<string> OnEvaluateJavaScriptRequested(string script)
        {
            var jsr = new JavascriptResult();
            Device.BeginInvokeOnMainThread(() =>
            {
                Control?.EvaluateJavascript(script, jsr);
            });

            var result = await jsr.JsResult;
            return result;
        }

        void OnGoBackRequested(object sender, EventArgs eventArgs)
        {
            if (Control.CanGoBack())
                Control.GoBack();

            UpdateCanGoBackForward();
        }

        void OnGoForwardRequested(object sender, EventArgs eventArgs)
        {
            if (Control.CanGoForward())
                Control.GoForward();

            UpdateCanGoBackForward();
        }

        protected internal void UpdateCanGoBackForward()
        {
            if (Element == null || Control == null)
                return;
            ElementController.CanGoBack = Control.CanGoBack();
            ElementController.CanGoForward = Control.CanGoForward();
        }

        void UpdateMixedContentMode()
        {
            if (Control != null && ((int)Build.VERSION.SdkInt >= 21))
            {
                Control.Settings.MixedContentMode = (MixedContentHandling)Element.On<Xamarin.Forms.PlatformConfiguration.Android>().MixedContentMode();
            }
        }

        public void OnInitializeComplete()
        {
            Element.OnInitializeComplete(new EventArgs());
        }

        public void OnGetContents(string title, string content, string action)
        {
            Element.OnGetContents(new HybridWebView.GetContentsEventArgs()
            {
                Title = title,
                Content = content,
                Action = action
            });
        }


        public void OnSaveEditorState(string state)
        {
            Element.OnSaveEditorState(new HybridWebView.SaveEditorStateEventArgs()
            {
                State = state
            });
        }

        public void OnShowImageDetails(string imageSource, string imageWidth, string imageHeight, string legend, string title, string alt, string alignment)
        {
            Element.OnShowImageSettings(new HybridWebView.ShowImageSettingsEventArgs()
            {
                ImageSource = imageSource,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight,
                Legend = legend,
                Title = title,
                Alt = alt,
                Alignment = alignment
            });
        }

        class JavascriptResult : Java.Lang.Object, IValueCallback
        {
            TaskCompletionSource<string> source;
            public Task<string> JsResult { get { return source.Task; } }

            public JavascriptResult()
            {
                source = new TaskCompletionSource<string>();
            }

            public void OnReceiveValue(Java.Lang.Object result)
            {
                string json = ((Java.Lang.String)result).ToString();
                source.SetResult(json);
            }
        }
    }
}