using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebviewFocusIssue.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WebviewFocusIssue.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public EditorToolbar EditorToolbar { get; set; }

        public MainPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object, string>(this, "ExecuteJS", (sender, arg) => {
                try
                {
                    Browser.Eval(arg);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }
    }
}
