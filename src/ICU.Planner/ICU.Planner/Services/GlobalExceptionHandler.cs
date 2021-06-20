
using Prism.Logging;
using ReactiveUI;

using System;

namespace ICU.Planner.Services
{
    public class GlobalExceptionHandler : IObserver<Exception>
    {
        private readonly ILogger logger;

        public GlobalExceptionHandler(ILogger logger)
        {
            this.logger = logger;
            RxApp.DefaultExceptionHandler = this;
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void OnNext(Exception ex)
        {
            logger.Log(ex);
        }
    }
}
