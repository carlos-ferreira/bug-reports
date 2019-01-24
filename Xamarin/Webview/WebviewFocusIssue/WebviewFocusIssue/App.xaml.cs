using Prism;
using Prism.Ioc;
using Prism.Unity;
using System;
using WebviewFocusIssue.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace WebviewFocusIssue
{
    public partial class App : PrismApplication
    {
        public App()
        {
        }

        public App(IPlatformInitializer initializer = null) : base(initializer)
        {

        }


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnInitialized()
        {
            InitializeComponent();
            NavigationService.NavigateAsync("MainPage");
         
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>("Navigation");
            containerRegistry.RegisterForNavigation<MasterDetailPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
        }
    }
}
