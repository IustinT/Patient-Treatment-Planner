using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
//using Serilog.Exceptions;
//using Serilog.Exceptions.Core;
//using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ICU.API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = GetConfiguration(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                //.Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                //.WithDefaultDestructurers()
                //.WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();

                Log.Information("Starting host...");

                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static IConfiguration GetConfiguration(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Environments.Development;

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"serilog.{environment}.json", optional: true, reloadOnChange: true);

            if (isDevelopment)
            {
                // configurationBuilder.AddUserSecrets<Startup>();
            }

            configurationBuilder.AddCommandLine(args);
            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder.Build();
        }
    }
}
