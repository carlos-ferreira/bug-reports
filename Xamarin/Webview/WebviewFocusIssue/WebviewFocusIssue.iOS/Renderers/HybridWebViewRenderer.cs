using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;
using WebKit;
using WebviewFocusIssue.Controls;
using WebviewFocusIssue.iOS.Controls;
using WebviewFocusIssue.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace WebviewFocusIssue.iOS.Renderers
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler, IWebViewDelegate
    {
        private WKUserContentController userController;
        bool _ignoreSourceChanges;
        WebNavigationEvent _lastBackForwardEvent;
        HybridWebView WebView => Element as HybridWebView;

        public override bool CanBecomeFirstResponder => true;

        public override bool CanResignFirstResponder => true;

        public UIView CustomInputAccessoryView { get; set; }
        public override UIView InputAccessoryView
        {
            get
            {
                return CustomInputAccessoryView;
            }
        }

        public UIInputViewController CustomInputAccessoryViewController { get; set; }

        public override UIInputViewController InputAccessoryViewController
        {
            get { return base.InputAccessoryViewController; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                userController = new WKUserContentController();
                userController.AddScriptMessageHandler(this, "BloggerPro");

                var config = new WKWebViewConfiguration
                {
                    UserContentController = userController,
                    Preferences = new WKPreferences()
                    {
                        JavaScriptEnabled = true
                    }
                };

                var webView = new CustomWebView(Frame, config)
                {
                    NavigationDelegate = new CustomWebViewDelegate(this),
                    AutosizesSubviews = true
                };

                if (Element.EditorToolbar != null)
                {
                    var rend = Platform.CreateRenderer(e.NewElement.EditorToolbar);
                    CustomInputAccessoryView = rend.NativeView;
                    
                }

                HideFormAccessoryBar.SetHideFormAccessoryBar(true);

                SetNativeControl(webView);
            }

            if (e.NewElement != null)
            {
                var newElement = e.NewElement;
                newElement.PropertyChanged += HandlePropertyChanged;
                newElement.EvalRequested += OnEvalRequested;
                newElement.EvaluateJavaScriptRequested += OnEvaluateJavaScriptRequested;
                newElement.EvaluateJavascript = async (js) =>
                {
                    var result = await Control.EvaluateJavaScriptAsync(js);

                    return result?.ToString();
                };

              

                Load();
            }

            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("BloggerPro");

                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.PropertyChanged -= HandlePropertyChanged;
                hybridWebView.EvalRequested -= OnEvalRequested;

               

                hybridWebView.Cleanup();
            }
            
        }

        

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            var rawMessage = message.Body.ToString();
            var jsonObject = JObject.Parse(rawMessage);

            var action = jsonObject.SelectToken("action").ToString();

            switch (action)
            {
                case "onInitializeComplete":
                    Element.OnInitializeComplete(new EventArgs());
                    break;

                case "onSaveState":
                    Element.OnSaveEditorState(new HybridWebView.SaveEditorStateEventArgs()
                    {
                        State = jsonObject.SelectToken("content").ToString()
                    });
                    break;

                case "onGetContents":
                    var content = JObject.Parse(jsonObject.SelectToken("content").ToString());

                    Element.OnGetContents(new HybridWebView.GetContentsEventArgs()
                    {
                        Title = content.SelectToken("title").ToString(),
                        Content = content.SelectToken("content").ToString(),
                        Action = content.SelectToken("action").ToString()
                    });
                    break;

                case "onShowImageDetails":
                    var showImageContent = JObject.Parse(jsonObject.SelectToken("content").ToString());
                    Element.OnShowImageSettings(new HybridWebView.ShowImageSettingsEventArgs()
                    {
                        ImageSource = showImageContent.SelectToken("imageSrc").ToString(),
                        ImageWidth = showImageContent.SelectToken("imageWidth").ToString(),
                        ImageHeight = showImageContent.SelectToken("imageHeight").ToString(),
                        Title = showImageContent.SelectToken("imageLegend").ToString(),
                        Legend = showImageContent.SelectToken("imageTitle").ToString(),
                        Alt = showImageContent.SelectToken("imageAlt").ToString(),
                        Alignment = showImageContent.SelectToken("alignment").ToString()
                    });
                    break;
            }
        }

        public void LoadHtml(string html, string baseUrl)
        {
            if (html != null)
                Control.LoadHtmlString(html, baseUrl == null ? new NSUrl(NSBundle.MainBundle.BundlePath, true) : new NSUrl(baseUrl, true));
        }

        public void LoadUrl(string url)
        {
            var uri = new Uri(url);
            var safeHostUri = new Uri($"{uri.Scheme}://{uri.Authority}", UriKind.Absolute);
            var safeRelativeUri = new Uri($"{uri.PathAndQuery}{uri.Fragment}", UriKind.Relative);
            Control.LoadRequest(new NSUrlRequest(new Uri(safeHostUri, safeRelativeUri)));
        }

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Source")
            {
                Load();
            }


        }

        void Load()
        {
            if (_ignoreSourceChanges)
                return;

            if (((WebView)Element).Source != null)
                ((WebView)Element).Source.Load(this);


            UpdateCanGoBackForward();
        }

        void OnEvalRequested(object sender, EvalRequested eventArg)
        {
            Control.EvaluateJavaScriptAsync(eventArg.Script);
        }

        async Task<string> OnEvaluateJavaScriptRequested(string script)
        {
            try
            {
                var result = await Control.EvaluateJavaScriptAsync(script);
                return result?.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        void OnGoBackRequested(object sender, EventArgs eventArgs)
        {
            if (Control.CanGoBack)
            {
                _lastBackForwardEvent = WebNavigationEvent.Back;
                Control.GoBack();
            }

            UpdateCanGoBackForward();
        }

        void OnGoForwardRequested(object sender, EventArgs eventArgs)
        {
            if (Control.CanGoForward)
            {
                _lastBackForwardEvent = WebNavigationEvent.Forward;
                Control.GoForward();
            }

            UpdateCanGoBackForward();
        }

        void UpdateCanGoBackForward()
        {
            ((IWebViewController)WebView).CanGoBack = Control.CanGoBack;
            ((IWebViewController)WebView).CanGoForward = Control.CanGoForward;
        }

        class CustomWebViewDelegate : WKNavigationDelegate
        {
            readonly HybridWebViewRenderer _renderer;
            WebNavigationEvent _lastEvent;

            public CustomWebViewDelegate(HybridWebViewRenderer renderer)
            {
                if (renderer == null)
                    throw new ArgumentNullException("renderer");
                _renderer = renderer;
            }

            HybridWebView WebView => _renderer.WebView;

            public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
            {
                var url = GetCurrentUrl();
                WebView.SendNavigated(
                    new WebNavigatedEventArgs(_lastEvent, new UrlWebViewSource { Url = url }, url, WebNavigationResult.Failure)
                );

                _renderer.UpdateCanGoBackForward();
            }

            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                if (webView.IsLoading)
                    return;

                _renderer._ignoreSourceChanges = true;
                var url = GetCurrentUrl();
                WebView.Source = new UrlWebViewSource { Url = url };
                _renderer._ignoreSourceChanges = false;

                var args = new WebNavigatedEventArgs(_lastEvent, WebView.Source, url, WebNavigationResult.Success);
                WebView.SendNavigated(args);

                _renderer.UpdateCanGoBackForward();
            }

            public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
            {
            }

            // https://stackoverflow.com/questions/37509990/migrating-from-uiwebview-to-wkwebview
            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                var navEvent = WebNavigationEvent.NewPage;
                var navigationType = navigationAction.NavigationType;
                switch (navigationType)
                {
                    case WKNavigationType.LinkActivated:
                        navEvent = WebNavigationEvent.NewPage;
                        break;
                    case WKNavigationType.FormSubmitted:
                        navEvent = WebNavigationEvent.NewPage;
                        break;
                    case WKNavigationType.BackForward:
                        navEvent = _renderer._lastBackForwardEvent;
                        break;
                    case WKNavigationType.Reload:
                        navEvent = WebNavigationEvent.Refresh;
                        break;
                    case WKNavigationType.FormResubmitted:
                        navEvent = WebNavigationEvent.NewPage;
                        break;
                    case WKNavigationType.Other:
                        navEvent = WebNavigationEvent.NewPage;
                        break;
                }

                _lastEvent = navEvent;
                var request = navigationAction.Request;
                var lastUrl = request.Url.ToString();
                var args = new WebNavigatingEventArgs(navEvent, new UrlWebViewSource { Url = lastUrl }, lastUrl);

                WebView.SendNavigating(args);
                _renderer.UpdateCanGoBackForward();
                decisionHandler(args.Cancel ? WKNavigationActionPolicy.Cancel : WKNavigationActionPolicy.Allow);
            }

            string GetCurrentUrl()
            {
                return _renderer.Control?.Url?.AbsoluteUrl?.ToString();
            }



        }

    }
}