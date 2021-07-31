using System;
using Prism;
using Prism.Ioc;
using Prism.Magician;
using Prism.Navigation;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

#if DEBUG
using System.Reflection;
using DMC.Views;
#endif



namespace DMC
{
    [AutoRegisterViews]
    public partial class App
    {
        public App(IPlatformInitializer platformInitializer) : base(platformInitializer) { }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterAppCenterLogger(
                "android=7f179bb3-6413-41a4-8f45-e0b444f27f89;" +
                  "ios=10b1ebf3-021b-4495-abb5-076ae2219236");

            containerRegistry.Register<IVersionTracking, VersionTrackingImplementation>();
            containerRegistry.Register<IMainThread, MainThreadImplementation>();
            containerRegistry.Register<IDeviceInfo, DeviceInfoImplementation>();
            containerRegistry.Register<IMediaPicker, MediaPickerImplementation>();
            containerRegistry.Register<IFileSystem, FileSystemImplementation>();

            base.RegisterRequiredTypes(containerRegistry);
        }

        protected override void OnInitialized()
        {


#if DEBUG
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                System.Diagnostics.Debug.WriteLine("############### found resource: " + res);
            }
#endif
            NavigationService
                .NavigateAsync(
                $"/{Navigation.NavigationKeys.NavigationPage}/{Navigation.NavigationKeys.MainPage}",
                new NavigationParameters())

                .OnNavigationError(OnNavigationError);
        }

        protected override void OnStart()
        {
            base.OnStart();

            var versionTracking = Container.Resolve<IVersionTracking>();
            if (versionTracking != null)
                versionTracking.Track();

        }

        private void OnNavigationError(Exception ex)
        {
            System.Diagnostics.Debugger.Break();
            Logger.Report(ex, null);

        }
    }
}
