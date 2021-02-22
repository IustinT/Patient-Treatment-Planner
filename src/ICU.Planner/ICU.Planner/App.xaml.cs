using Flurl.Http;
using Flurl.Http.Configuration;

using ICU.Planner.Polly;
using ICU.Planner.Views;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Prism;
using Prism.Ioc;
using Prism.Magician;
using Prism.Services;

using Shiny.Logging;

using System;
using System.Reflection;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;


namespace ICU.Planner
{
    [AutoRegisterViews]
    public partial class App
    {
        public App(IPlatformInitializer platformInitializer) : base(platformInitializer) { }

        protected override async void OnInitialized()
        {
#if DEBUG
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                System.Diagnostics.Debug.WriteLine("############### found resource: " + res);
            }
#endif
            await NavigationService.NavigateAsync($"/{Navigation.NavigationKeys.NavigationPage}/{Navigation.NavigationKeys.MainPage}");
        }

        protected override void OnStart()
        {
            base.OnStart();

            var versionTracking = Container.Resolve<IVersionTracking>();
            if (versionTracking != null)
                versionTracking.Track();

            FlurlHttp.Configure(FlurlConfigAction);
        }

        private void FlurlConfigAction(GlobalFlurlHttpSettings settings)
        {
            settings.HttpClientFactory = new PollyHttpClientFactory();

            settings.JsonSerializer = new NewtonsoftJsonSerializer(Constants.FlurlHttp.JsonSettings);

            settings.OnErrorAsync += call => Log.SafeExecute(() => OnFlurlErrorAsync(call));
            settings.AfterCall += (call) => { };
        }

        private async Task OnFlurlErrorAsync(FlurlCall httpCall)
        {
            Log.Write(httpCall.Exception,
                 ("action", nameof(OnFlurlErrorAsync)),
                ("RequestUri", httpCall.Request.Url?.ToString()),
                ("RequestMethod", httpCall.Request.Verb?.ToString()),
                ("HttpStatus", httpCall.Response?.StatusCode.ToString()),
                ("ResponseMessage", httpCall.Response?.ResponseMessage?.ToString())
            );

            if (httpCall.Response?.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
            {
                //here the user should be logged out the application, if logging in ever gets implemented

                httpCall.ExceptionHandled = true;
                // throw an exception so the execution chain gets interrupted
                throw new Exception("You must sign in.");
            }
            else if (httpCall.Response?.StatusCode == (int)System.Net.HttpStatusCode.BadRequest
                || httpCall.Response?.StatusCode == (int)System.Net.HttpStatusCode.Conflict)
            {
                var pageDialogService = Container.Resolve<IPageDialogService>();
                if (pageDialogService != null)
                {
                    var resultContent = await httpCall.Response.GetStringAsync();
                    var message = $@"A network request failed with the status ""{httpCall.HttpResponseMessage?.ReasonPhrase}"".";

                    if (!string.IsNullOrWhiteSpace(resultContent))
                    {
                        message += $"{Environment.NewLine}The text below might provide more info:{Environment.NewLine}";
                        try
                        {
                            // sometimes the result would be JSON
                            message += JToken.Parse(resultContent).ToString(Formatting.Indented);
                        }
                        catch
                        {
                            // sometimes the result won't be JSON
                            message += resultContent;
                        }
                    }

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await pageDialogService.DisplayAlertAsync("Error", message, "Close");
                    });
                }
            }
        }

    }
}
