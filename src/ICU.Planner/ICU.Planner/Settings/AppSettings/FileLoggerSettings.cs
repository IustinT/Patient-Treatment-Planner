using Playground.Forms.Settings.AppSettings;

namespace ICU.Planner.Settings.AppSettings
{
    public class FileLoggerSettings : AppSettingsBase
    {
        
        public string LogFileName { get; private set; }
        public int LogFileMaxLines { get; private set; }
    }
}
