using System;
using System.Collections.Generic;
using System.Text;
using WebviewFocusIssue.Views;
using Xamarin.Forms;

namespace WebviewFocusIssue.Controls
{
    [ContentProperty("Template")]
    public class EditorToolbarBehavior : Behavior<MainPage>
    {
        public MainPage AssociatedObject { get; private set; }

        protected override void OnAttachedTo(MainPage bindable)
        {

            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            if (bindable != null)
            {
                bindable.EditorToolbar = Template as EditorToolbar;
                ((MainPage)bindable).BindingContextChanged += OnBindingContextChanged;
            }
        }

        protected override void OnDetachingFrom(MainPage bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            AssociatedObject = null;
        }

        void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }

        public ContentView Template { get; set; }
    }
}
