﻿using Essential.Diagnostics;
using System;
using System.Diagnostics;
using System.IO;

namespace TrainNotifier.Common
{
    public static class TraceHelper
    {
        private static readonly RollingFileTraceListener _rollingFileTraceListener = new RollingFileTraceListener
        {
            ConvertWriteToEvent = true,
            Template = "{DateTime:HH':'mm':'ssZ}: {Message}{Data}"
        };

        /// <summary>
        /// flush the trace log
        /// </summary>
        public static void FlushLog()
        {
            if (_rollingFileTraceListener != null)
            {
                _rollingFileTraceListener.Flush();
            }
        } 

        /// <summary>
        /// set up the trace listener to write to the "Logs" sub-directory
        /// </summary>
        public static void SetupTrace()
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            Directory.SetCurrentDirectory(logPath);

            Trace.Listeners.Add(new ColoredConsoleTraceListener
            {
                Template = "{DateTime:HH':'mm':'ssZ}: {Message}{Data}"
            });
            Trace.Listeners.Add(_rollingFileTraceListener);
        }
    }
}
