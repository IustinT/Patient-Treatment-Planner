using System;
using System.IO;
using System.Linq;

using ICU.Planner.Settings.AppSettings;

using Microsoft.Extensions.Options;

using Shiny.Logging;

using Xamarin.Essentials.Interfaces;

namespace ICU.Planner.Logging
{
    public class LimitedFileLogger : ILogger
    {
        private readonly object _syncLock = new object();
        private readonly string _filePath;
        private readonly int _maxLines;

        public LimitedFileLogger(IOptions<FileLoggerSettings> fileLoggerSettings, IFileSystem fileSystem)
        { 
            _filePath = Path.Combine(fileSystem.AppDataDirectory, fileLoggerSettings.Value.LogFileName); 
            _maxLines = fileLoggerSettings.Value.LogFileMaxLines;
        }

        public void Write(Exception exception, params (string Key, string Value)[] parameters)
        {
            var message = $@"[{DateTime.Now:MM/d/yyyy hh:mm:ss tt}] {exception}";

            Write(message);
        }


        public void Write(string eventName, string description, params (string Key, string Value)[] parameters)
        {
            var message = $@"[{DateTime.Now:MM/d/yyyy hh:mm:ss tt}] {eventName}";
            if (!string.IsNullOrWhiteSpace(description))
                message += $" - {description}";

            Write(message);
        }

        private void Write(string message)
        {
            lock (_syncLock)
            {
                if (!File.Exists(_filePath))
                {
                    File.AppendAllText(_filePath, message);
                    return;
                }

                var logEntries = File.ReadAllLines(_filePath).ToList();

                logEntries.Add(message);

                while (logEntries.Count > _maxLines)
                    logEntries.RemoveAt(0);

                File.WriteAllLines(_filePath, logEntries);
            }
        }
    }
}
