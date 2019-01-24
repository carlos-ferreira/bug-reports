using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace WebviewFocusIssue.Controls
{
    public class HybridWebView : WebView
    {
        #region Bindable Properties

        public static BindableProperty EvaluateJavascriptProperty =
        BindableProperty.Create(nameof(EvaluateJavascript), typeof(Func<string, Task<string>>), typeof(HybridWebView), null, BindingMode.OneWayToSource);

        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return (Func<string, Task<string>>)GetValue(EvaluateJavascriptProperty); }
            set { SetValue(EvaluateJavascriptProperty, value); }
        }


        #endregion

        public EditorToolbar EditorToolbar { get; set; }

        Action<string> action;

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }

        public event EventHandler InitializeComplete;

        public event EventHandler<SaveEditorStateEventArgs> SaveEditorStateCompleted;

        public event EventHandler<GetContentsEventArgs> GetContentsCompleted;

        public event EventHandler<ShowImageSettingsEventArgs> ShowImageSettingsRequested;

        public virtual void OnInitializeComplete(EventArgs e)
        {
            InitializeComplete?.Invoke(this, e);
        }

        public virtual void OnSaveEditorState(SaveEditorStateEventArgs e)
        {
            SaveEditorStateCompleted?.Invoke(this, e);
        }

        public virtual void OnGetContents(GetContentsEventArgs e)
        {
            GetContentsCompleted?.Invoke(this, e);
        }

        public virtual void OnShowImageSettings(ShowImageSettingsEventArgs e)
        {
            ShowImageSettingsRequested?.Invoke(this, e);
        }

        public class SaveEditorStateEventArgs : EventArgs
        {
            public string State { get; set; }
        }

        public class GetContentsEventArgs : EventArgs
        {
            public string Title { get; set; }
            public string Content { get; set; }
            public string Action { get; set; }
        }

        public class ShowImageSettingsEventArgs : EventArgs
        {
            public string ImageSource { get; set; }
            public string ImageWidth { get; set; }
            public string ImageHeight { get; set; }
            public string Legend { get; set; }
            public string Title { get; set; }
            public string Alt { get; set; }
            public string Alignment { get; set; }
        }
    }
}
