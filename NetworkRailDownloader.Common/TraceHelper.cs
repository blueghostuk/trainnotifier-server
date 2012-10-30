using Essential.Diagnostics;
using System;
using System.Diagnostics;
using System.IO;

namespace NetworkRailDownloader.Common
{
    public static class TraceHelper
    {
        public static void SetupTrace()
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            Directory.SetCurrentDirectory(logPath);

            Trace.Listeners.Add(new ColoredConsoleTraceListener
            {
                Template = "{DateTime:HH':'mm':'ssZ} [{Thread}] {EventType}: {Message}{Data}"
            });
            Trace.Listeners.Add(new RollingFileTraceListener
            {
                ConvertWriteToEvent = true,
                Template = "{DateTime:HH':'mm':'ssZ} [{Thread}] {EventType}: {Message}{Data}"
            });
        }
    }
}
