using ICU.Planner.Modules;
using ICU.Planner.Services;

using Microsoft.Extensions.DependencyInjection;

using Shiny;
using Shiny.Jobs;
using Shiny.Logging;

using System;

namespace ICU.Planner
{
    public partial class Startup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // Add Xamarin Essentials
            services.RegisterModule<EssentialsModule>();

            // Add Settings
            services.RegisterModule<SettingsModule>();

            // Add Loggers
            services.RegisterModule<LoggingModule>();
            services.AddSingleton<GlobalExceptionHandler>();

        }

    }
}
