using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using WebviewFocusIssue;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MainPage), typeof(WebviewFocusIssue.iOS.Renderers.IosPageRenderer))]

namespace WebviewFocusIssue.iOS.Renderers
{
    public class IosPageRenderer : PageRenderer
    {
        public override bool CanBecomeFirstResponder => true;

        public override bool CanResignFirstResponder => true;


        List<ToolbarItem> _secondaryItems;
        UITableView table;

        NSObject observerHideKeyboard;
        NSObject observerShowKeyboard;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.NewElement is MainPage)
            {
                var page = e.NewElement as MainPage;
                _secondaryItems = page.ToolbarItems.Where(i => i.Order == ToolbarItemOrder.Secondary).ToList();
                _secondaryItems.ForEach(t => page.ToolbarItems.Remove(t));
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            var element = (ContentPage)Element;

            var hasMoreToolbarIcon = element.ToolbarItems.Any(ti => ti.Icon == "more.png");

            if (_secondaryItems != null && _secondaryItems.Count > 0 && !hasMoreToolbarIcon)
            {
                element.ToolbarItems.Add(new ToolbarItem()
                {
                    Order = ToolbarItemOrder.Primary,
                    Icon = "more.png",
                    Priority = 1,
                    Command = new Command(() =>
                    {
                        ToolClicked();
                    })
                });
            }

            observerHideKeyboard = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            observerShowKeyboard = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);

            base.ViewWillAppear(animated);

        }

        public override void ViewWillDisappear(bool animated)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(observerHideKeyboard);
            NSNotificationCenter.DefaultCenter.RemoveObserver(observerShowKeyboard);

            base.ViewWillDisappear(animated);
        }

        void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded) return;
            var frameBegin = UIKeyboard.FrameBeginFromNotification(notification);
            var frameEnd = UIKeyboard.FrameEndFromNotification(notification);
            var bounds = Element.Bounds;
            var newBounds = new Xamarin.Forms.Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height - frameBegin.Top + frameEnd.Top);
            Element.Layout(newBounds);

            this.replaceKeyboardInputAccessoryView();
        }

        void addNewAccessoryView(UIView oldAccessoryView)
        {
            var frame = oldAccessoryView.Frame;
            var newAccessoryView = new UIView(frame);
            //newAccessoryView.BackgroundColor = UIColor.LightGray;

            //var fn = ".HelveticaNeueInterface-Bold";
            //var doneButton = new UIButton(new CGRect(frame.Size.Width - 80, 6, 100, 30));

            //doneButton.SetTitle("Done", forState: UIControlState.Normal);
            //doneButton.SetTitleColor(UIColor.Blue, forState: UIControlState.Normal);
            //doneButton.TitleLabel.Font = UIFont.FromName(fn, 15);
            //doneButton.AddTarget(this, new ObjCRuntime.Selector("buttonAccessoryDoneAction:"), UIControlEvent.TouchUpInside);

            //var prevButton = new UIButton(new CGRect(2, 6, 50, 30));
            //prevButton.SetTitle("<", forState: UIControlState.Normal);
            //prevButton.SetTitleColor(UIColor.Blue, forState: UIControlState.Normal);
            //prevButton.TitleLabel.Font = UIFont.FromName(fn, 15);
            //prevButton.AddTarget(this, new ObjCRuntime.Selector("buttonAccessoryPrevAction:"), UIControlEvent.TouchUpInside);

            //var nextButton = new UIButton(new CGRect(35, 6, 50, 30));
            //nextButton.SetTitle(">", UIControlState.Normal);
            //nextButton.SetTitleColor(UIColor.Blue, UIControlState.Normal);
            //nextButton.TitleLabel.Font = UIFont.FromName(fn, 15);
            //nextButton.AddTarget(this, new ObjCRuntime.Selector("buttonAccessoryNextAction:"), UIControlEvent.TouchUpInside);
            var page = Element as EditorPage;
            if (page.EditorToolbar != null)
            {
                var rend = Platform.CreateRenderer(page.EditorToolbar);
                
                UIView view = rend.NativeView;
                view.LayoutSubviews();

                this.View.AddSubview(view);
                
                newAccessoryView.AddSubview(view);
            }
            //newAccessoryView.AddSubview(nextButton);
            //newAccessoryView.AddSubview(doneButton);

            oldAccessoryView.AddSubview(newAccessoryView);
        }

        UIView traverseSubViews(UIView vw)
        {
            if (vw.Description.StartsWith("<UIWebFormAccessory"))
            {
                return vw;
            }

            for (var i = 0; i < vw.Subviews.Count(); i++)
            {
                var subview = vw.Subviews[i] as UIView;
                var inputAccessoryView=subview.InputAccessoryView;
                if (subview.Subviews.Count() > 0)
                {
                    var subvw = this.traverseSubViews(subview);
                    if (subvw.Description.StartsWith("<UIWebFormAccessory"))
                    {
                        return subvw;
                    }
                }
            }

            return new UIView();
        }

        void replaceKeyboardInputAccessoryView()
        {
            // locate accessory view
            var windowCount = UIApplication.SharedApplication.Windows.Count();
            if (windowCount < 2)
            {
                return;
            }

            foreach (UIWindow window in UIApplication.SharedApplication.Windows)
            {
                UIView accessoryView = traverseSubViews(window);
                if (accessoryView.Description.StartsWith("<UIWebFormAccessory"))
                {
                    // Found the inputAccessoryView UIView
                    if (accessoryView.Subviews.Count() > 0)
                    {
                        this.addNewAccessoryView(accessoryView);
                    }
                }
            }
           
        }

        // Override these in your UIViewController
        void buttonAccessoryNextAction(UIButton sender)
        {
            Console.WriteLine("Next Clicked");
        }

        void buttonAccessoryPrevAction(UIButton sender)
        {
            Console.WriteLine("Prev Clicked");
        }

        void buttonAccessoryDoneAction(UIButton sender)
        {
            Console.WriteLine("Done Clicked");
        }

        

        //Create a table instance and added it to the view.
        private void ToolClicked()
        {
            if (table == null)
            {
                var childRect = new RectangleF((float)View.Bounds.Width - 250, 7, 250, _secondaryItems.Count() * 56);
                table = new UITableView(childRect)
                {
                    Source = new TableSource(_secondaryItems),
                    ClipsToBounds = false
                };
                table.Layer.ShadowColor = UIColor.Black.CGColor;
                table.Layer.ShadowOpacity = 0.3f;
                table.Layer.ShadowRadius = 5.0f;
                table.Layer.ShadowOffset = new System.Drawing.SizeF(5f, 5f);
                table.BackgroundColor = UIColor.White;
                Add(table);
                return;
            }

            foreach (var subview in View.Subviews)
            {
                if (subview == table)
                {
                    table.RemoveFromSuperview();
                    return;
                }
            }
            Add(table);
        }

    }
}