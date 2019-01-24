using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace WebviewFocusIssue.Converters
{
    public class HtmlSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            WebViewSource webviewSource = null;

            if (value == null)
                return webviewSource;


            webviewSource = new UrlWebViewSource()
            {
                Url = value.ToString()
            };

            return webviewSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
