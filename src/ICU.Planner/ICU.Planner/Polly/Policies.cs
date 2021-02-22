
using Polly;
using Polly.Retry;
using Polly.Timeout;

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ICU.Planner.Polly
{
    public static class Policies
    {
        public static AsyncTimeoutPolicy<HttpResponseMessage> TimeoutPolicy
        {
            get
            {
                return Policy.TimeoutAsync<HttpResponseMessage>(15, (context, timeSpan, task) =>
                {
                    Debug.WriteLine($"[App|Policy]: Timeout delegate fired after {timeSpan.TotalSeconds} seconds");
                    return Task.CompletedTask;
                });
            }
        }

        public static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy
        {
            get
            {
                return Policy
                    .HandleResult<HttpResponseMessage>(r =>
                        !r.IsSuccessStatusCode
                        && r.StatusCode != HttpStatusCode.BadRequest
                        && r.StatusCode != HttpStatusCode.Conflict
                    )
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(new[]
                        {
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(7),
                            TimeSpan.FromSeconds(10)
                        },
                        (delegateResult, timeSpan) =>
                        {
                            Debug.WriteLine(
                                $"[App|Policy]: Retry delegate fired, waiting: {timeSpan}");
                        });
            }
        }
    }
}