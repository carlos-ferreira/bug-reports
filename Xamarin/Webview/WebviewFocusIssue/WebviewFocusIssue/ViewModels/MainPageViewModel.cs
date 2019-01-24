using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using WebviewFocusIssue.Services.Webview;
using Xamarin.Forms;

namespace WebviewFocusIssue.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private readonly IBaseUrlProvider _baseUrlProvider;

        public DelegateCommand PageLoadedCommand { get; set; }
        public DelegateCommand ToggleBoldCommand { get; set; }

        private string _editorContent;
        public string EditorContent
        {
            get { return _editorContent; }
            set { SetProperty(ref _editorContent, value); }
        }

        public MainPageViewModel(IBaseUrlProvider baseUrlProvider)
        {
            PageLoadedCommand = new DelegateCommand(PageLoadCompleted);
            ToggleBoldCommand = new DelegateCommand(ToggleBold);
            _baseUrlProvider = baseUrlProvider;

            EditorContent = string.Format("{0}/Editor/editor.html", _baseUrlProvider.GetBaseUrl());
        }

        private void PageLoadCompleted()
        {
            MessagingCenter.Send<object, string>(this, "ExecuteJS", string.Format("BPEditor.initialize('{0}','{1}');","Title", "Enter your content"));

        }

        private void ToggleBold()
        {
           MessagingCenter.Send<object, string>(this, "ExecuteJS", "BPEditor.toggleBold();");
        }

        

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
