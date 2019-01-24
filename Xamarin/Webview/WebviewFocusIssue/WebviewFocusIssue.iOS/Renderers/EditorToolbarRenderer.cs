using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using WebviewFocusIssue.Controls;
using WebviewFocusIssue.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EditorToolbar), typeof(EditorToolbarRenderer))]

namespace WebviewFocusIssue.iOS.Renderers
{
    public class EditorToolbarRenderer: VisualElementRenderer<ContentView>
    {
    }
}