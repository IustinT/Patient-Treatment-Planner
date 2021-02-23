
using Microsoft.Extensions.DependencyInjection;

using Shiny;

using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace ICU.Planner.Modules
{
    public class EssentialsModule : ShinyModule
    {
        public override void Register(IServiceCollection services)
        {
            // Add Xamarin Essentials
            services.AddTransient<IAppInfo, AppInfoImplementation>();
            services.AddTransient<ILauncher, LauncherImplementation>();
            services.AddTransient<IFileSystem, FileSystemImplementation>();
            services.AddTransient<IBrowser, BrowserImplementation>();
            services.AddTransient<IConnectivity, ConnectivityImplementation>();
            services.AddTransient<IBattery, BatteryImplementation>();
            services.AddTransient<IDeviceDisplay, DeviceDisplayImplementation>();
            services.AddTransient<IDeviceInfo, DeviceInfoImplementation>();
            services.AddTransient<IMainThread, MainThreadImplementation>();
            services.AddTransient<IPhoneDialer, PhoneDialerImplementation>();
            services.AddTransient<ISecureStorage, SecureStorageImplementation>();
            services.AddTransient<IOrientationSensor, OrientationSensorImplementation>();
            services.AddTransient<IPermissions, PermissionsImplementation>();
            services.AddTransient<IVersionTracking, VersionTrackingImplementation>();
            services.AddTransient<IWebAuthenticator, WebAuthenticatorImplementation>();
            services.AddTransient<IVersionTracking, VersionTrackingImplementation>();
            services.AddTransient<IMediaPicker, MediaPickerImplementation>();
            //services.AddTransient<IEmail, EmailImplementation>();
            //services.AddTransient<ISms, SmsImplementation>();
            // and so on...
        }
    }
}
