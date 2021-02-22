using System.Reflection;

using ICU.Planner.Settings.AppSettings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Shiny;

namespace ICU.Planner.Modules
{
    public class SettingsModule : ShinyModule
    {
        public override void Register(IServiceCollection services)
        {
            // AppSettings (loaded from embedded json settings files to readonly properties)
            string path = $"{typeof(App).Namespace}.Settings.AppSettings.SettingsFiles.appsettings.json";

            var stream = Assembly
                .GetAssembly(typeof(App))
                .GetManifestResourceStream(path);

            if (stream != null)
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

                // Add all settings sections here
                services.Configure<AppCenterSettings>(config.GetSection($"Logging:{nameof(AppCenterSettings)}"), options => options.BindNonPublicProperties = true);

                services.Configure<FileLoggerSettings>(config.GetSection($"Logging:{nameof(FileLoggerSettings)}"), options => options.BindNonPublicProperties = true);

                services.AddOptions<SomeAppSettings>()
                    .Bind(config.GetSection(nameof(SomeAppSettings)), options => options.BindNonPublicProperties = true)
                    .ValidateDataAnnotations();
            }

            // UserSettings (sync with device preferences)
            //services.AddOptions<UserAccountSettings>(); 

        }
    }
}
