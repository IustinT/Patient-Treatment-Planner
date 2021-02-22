using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Options;
using Shiny;

using ICU.Planner.Settings.AppSettings;
using Xamarin.Essentials.Interfaces;

namespace ICU.Planner.Logging
{
    public class AppCenterLogger : IExtendedLogger
    {
        private readonly string _filePath;
        private readonly string _fileName;

        public AppCenterLogger(IOptions<AppCenterSettings> appCenterSettings, IOptions<FileLoggerSettings> fileLoggerSettings, IFileSystem fileSystem)
        {
            _filePath = Path.Combine(fileSystem.AppDataDirectory, fileLoggerSettings.Value.LogFileName);
            _fileName = fileLoggerSettings.Value.LogFileName;

            var list = new List<Type>(2);
            if (appCenterSettings.Value.TrackCrashes)
            {
                list.Add(typeof(Crashes));
                IsCrashEnabled = true;
            }

            if (appCenterSettings.Value.TrackEvents)
            {
                list.Add(typeof(Analytics));
                IsEventsEnabled = true;
            }

            AppCenter.Start(appCenterSettings.Value.Secret, list.ToArray());

            Crashes.GetErrorAttachments = GetErrorAttachments;
        }

        public bool IsCrashEnabled { get; }
        public bool IsEventsEnabled { get; }

        private IEnumerable<ErrorAttachmentLog> GetErrorAttachments(ErrorReport report)
        {
            var attachments = new List<ErrorAttachmentLog>();
            var attachment = GetErrorAttachment();
            if (attachment != null)
                attachments.Add(attachment);

            return attachments;
        }

        private ErrorAttachmentLog GetErrorAttachment()
        {
            ErrorAttachmentLog attachment = null;
            try
            {
                if (File.Exists(_filePath))
                {
                    var logBytes = File.ReadAllBytes(_filePath);
                    attachment = ErrorAttachmentLog.AttachmentWithBinary(logBytes, _fileName, "text/plain");
                }
            }
            catch (Exception ex)
            {
                attachment = ErrorAttachmentLog.AttachmentWithText($"Reading log file thrown error: {ex}", _fileName);
            }

            return attachment;
        }

        public void Write(Exception exception, params (string Key, string Value)[] parameters)
        {
            var attachment = GetErrorAttachment();
            Crashes.TrackError(exception, parameters?.ToDictionary(), attachment);
        }

        public void Write(string eventName, string description, params (string Key, string Value)[] parameters)
        {
            Analytics.TrackEvent($"[{eventName}] {description}", parameters.ToDictionary());
        }
    }
}
