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

namespace WebviewFocusIssue.Droid.Renderers
{
    public interface IWebViewBridge
    {
        void OnInitializeComplete();

        void OnSaveEditorState(string state);

        void OnGetContents(string title, string content, string action);

        void OnShowImageDetails(string imageSource,
                                string imageWidth,
                                string imageHeight,
                                string legend,
                                string title,
                                string alt,
                                string alignment);
    }
}