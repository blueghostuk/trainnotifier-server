using Apache.NMS;
using System;
using System.Diagnostics;
using System.Threading;

namespace TrainNotifier.Common.NMS
{
    public class NMSConnectionMonitor : IDisposable
    {
        private DateTime? _lastMsgRecd;
        private Timer _timer;

        public DateTime? LastMsgRecd
        {
            get { return _lastMsgRecd; }
        }

        public bool QuitOk { get; set; }

        private NMSConnectionMonitor()
        {
            QuitOk = true;
        }

        public static NMSConnectionMonitor MonitorConnection(IConnection connection, IMessageConsumer consumer,
            AutoResetEvent resetEvent)
        {
            NMSConnectionMonitor monitor = new NMSConnectionMonitor();
            monitor.MonitorConnectionLocal(connection, consumer, resetEvent);
            return monitor;
        }

        private void MonitorConnectionLocal(IConnection connection, IMessageConsumer consumer, AutoResetEvent resetEvent)
        {
            consumer.Listener += (m) =>
            {
                _lastMsgRecd = DateTime.UtcNow;
            };

            connection.ExceptionListener += (e) =>
            {
                Trace.TraceError("Connection error: {0}", e);
                QuitOk = false;
                resetEvent.Set();
            };
        }

        public void Dispose()
        {
            if (_timer != null)
                _timer.Dispose();
        }
    }
}
