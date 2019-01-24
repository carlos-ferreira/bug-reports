using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Webkit;
using Xamarin.Forms.Platform.Android;
using Object = Java.Lang.Object;

namespace WebviewFocusIssue.Droid.Renderers
{
    public class CustomWebChromeClient : FormsWebChromeClient
    {
        protected Activity _activity;

        internal void SetContext(Activity thisActivity)
        {
            if (thisActivity == null)
                throw new ArgumentNullException(nameof(thisActivity));

            _activity = thisActivity;
        }


    }
}