using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;
using Xamarin.Forms;

namespace ICU.Planner.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        NSObject didChangeStatusBarOrientationNotification;
        NSObject didBecomeActiveNotification;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();


            LoadApplication(new App(new iOSInitializer()));

            didChangeStatusBarOrientationNotification =
                UIApplication.Notifications
                .ObserveDidChangeStatusBarOrientation((sender, args) => OnSafeAreasChanged());

            didBecomeActiveNotification =
                UIApplication.Notifications
                .ObserveDidBecomeActive((sender, args) => OnSafeAreasChanged());

            return base.FinishedLaunching(app, options);
        }

        private void OnSafeAreasChanged()
        {
            var sharedApplication = UIApplication.SharedApplication;
            var interfaceOrientation = sharedApplication?.StatusBarOrientation ?? UIInterfaceOrientation.Unknown;
            var safeAreaInsets = sharedApplication?.KeyWindow?.SafeAreaInsets;

            if (safeAreaInsets is null) return;

            ResourceDictionary resources = Xamarin.Forms.Application.Current.Resources;

            SetSafeAreasToAppResources(safeAreaInsets.Value, resources);

            SetSafeAreasDependandAppResources(safeAreaInsets.Value, resources, interfaceOrientation);

        }

        private static void SetSafeAreasDependandAppResources(UIEdgeInsets safeAreaInsets, ResourceDictionary resources, UIInterfaceOrientation interfaceOrientation)
        {
            var mainContentPaddingDefault = resources[Constants.ResourceKeys.MainContentPaddingDefaultResourceName] as Thickness?;
            var dialogHeaderPaddingDefault = resources[Constants.ResourceKeys.DialogHeaderPaddingDefaultResourceName] as Thickness?;


            switch (interfaceOrientation)
            {
                case UIInterfaceOrientation.LandscapeLeft:

                    resources[Constants.ResourceKeys.DialogHeaderHeigthResourceName] = resources[Constants.ResourceKeys.DialogHeaderLandscapeHeigthResourceName];

                    resources[Constants.ResourceKeys.DialogHeaderPaddingResourceName] = new Thickness(
                        safeAreaInsets.Left,
                        dialogHeaderPaddingDefault?.Top ?? 0,
                        safeAreaInsets.Right,
                        dialogHeaderPaddingDefault?.Bottom ?? 0);

                    resources[Constants.ResourceKeys.MainContentPaddingResourceName] = new Thickness(

                        mainContentPaddingDefault?.Left ?? 0,
                        mainContentPaddingDefault?.Top ?? 0,
                        safeAreaInsets.Right,
                        mainContentPaddingDefault?.Bottom ?? 0);

                    break;

                case UIInterfaceOrientation.LandscapeRight:

                    resources[Constants.ResourceKeys.DialogHeaderHeigthResourceName] = resources[Constants.ResourceKeys.DialogHeaderLandscapeHeigthResourceName];

                    resources[Constants.ResourceKeys.DialogHeaderPaddingResourceName] = new Thickness(
                        safeAreaInsets.Left,
                        dialogHeaderPaddingDefault?.Top ?? 0,
                        safeAreaInsets.Right,
                        dialogHeaderPaddingDefault?.Bottom ?? 0);

                    resources[Constants.ResourceKeys.MainContentPaddingResourceName] = new Thickness(

                        safeAreaInsets.Left,
                        mainContentPaddingDefault?.Top ?? 0,
                        mainContentPaddingDefault?.Right ?? 0,
                        mainContentPaddingDefault?.Bottom ?? 0);

                    break;

                case UIInterfaceOrientation.Unknown:
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                default:

                    resources[Constants.ResourceKeys.DialogHeaderHeigthResourceName] = resources[Constants.ResourceKeys.DialogHeaderDefaultHeigthResourceName];
                    resources[Constants.ResourceKeys.DialogHeaderPaddingResourceName] = resources[Constants.ResourceKeys.DialogHeaderPaddingDefaultResourceName];
                    resources[Constants.ResourceKeys.MainContentPaddingResourceName] = resources[Constants.ResourceKeys.MainContentPaddingDefaultResourceName];
                    break;
            }
        }

        private static void SetSafeAreasToAppResources(UIEdgeInsets safeAreaInsets, ResourceDictionary resources)
        {
            resources[Constants.ResourceKeys.SafeAreaInsetsResourceName] =
                new Thickness(safeAreaInsets.Left, safeAreaInsets.Top, safeAreaInsets.Right, safeAreaInsets.Bottom);

            resources[Constants.ResourceKeys.SafeAreaInsetsTopResourceName] = safeAreaInsets.Top;
            resources[Constants.ResourceKeys.SafeAreaInsetsBottomResourceName] = safeAreaInsets.Bottom;
            resources[Constants.ResourceKeys.SafeAreaInsetsLeftResourceName] = safeAreaInsets.Left;
            resources[Constants.ResourceKeys.SafeAreaInsetsRightResourceName] = safeAreaInsets.Right;

            resources[Constants.ResourceKeys.SafeAreaInsetsVerticalResourceName] = safeAreaInsets.Top + safeAreaInsets.Bottom;
            resources[Constants.ResourceKeys.SafeAreaInsetsHorizontalResourceName] = safeAreaInsets.Left + safeAreaInsets.Right;
        }

        protected override void Dispose(bool disposing)
        {
            // Stop listening for display change
            if (disposing)
            {
                didChangeStatusBarOrientationNotification.Dispose();
                didBecomeActiveNotification.Dispose();
            }
            base.Dispose(disposing);

        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}
