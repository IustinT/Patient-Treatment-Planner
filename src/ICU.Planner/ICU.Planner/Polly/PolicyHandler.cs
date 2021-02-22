
using Polly;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ICU.Planner.Polly
{
    public class PolicyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // use the timeout policy if this is a GET http request
            if (request.Method == HttpMethod.Get)
            {
                return Policy
                    .WrapAsync(Policies.RetryPolicy, Policies.TimeoutPolicy)
                    .ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
            }

            return Policies
                .RetryPolicy
                .ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}