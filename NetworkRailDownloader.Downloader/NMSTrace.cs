using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.NMS
{
    internal class NMSTrace : ITrace
    {
        private static readonly bool _log = bool.Parse(ConfigurationManager.AppSettings["NMSLogging"]);

        public void Debug(string message)
        {
            Trace.TraceInformation(message);
        }

        public void Error(string message)
        {
            Trace.TraceError(message);
        }

        public void Fatal(string message)
        {
            Trace.TraceError(message);
        }

        public void Info(string message)
        {
            Trace.TraceInformation(message);
        }

        public bool IsDebugEnabled
        {
            get { return _log; }
        }

        public bool IsErrorEnabled
        {
            get { return _log; }
        }

        public bool IsFatalEnabled
        {
            get { return _log; }
        }

        public bool IsInfoEnabled
        {
            get { return _log; }
        }

        public bool IsWarnEnabled
        {
            get { return _log; }
        }

        public void Warn(string message)
        {
            Trace.TraceInformation(message);
        }
    }
}
