
using Shiny.Logging;

namespace ICU.Planner.Logging
{
    public interface IExtendedLogger : ILogger
    {
        bool IsCrashEnabled { get; }

        bool IsEventsEnabled { get; }
    }
}
