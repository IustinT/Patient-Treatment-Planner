
using ReactiveUI;

using Shiny;
using Shiny.Logging;

using System;

namespace ICU.Planner.Services
{
    public class GlobalExceptionHandler : IObserver<Exception>, IShinyStartupTask
    {
        public GlobalExceptionHandler() { }


        public void Start() => RxApp.DefaultExceptionHandler = this;
        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void OnNext(Exception ex)
        {
            Log.Write(ex);
        }
    }
}
