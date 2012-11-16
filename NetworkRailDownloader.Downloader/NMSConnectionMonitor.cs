using Apache.NMS;
using System;
using System.Diagnostics;
using System.Threading;

namespace TrainNotifier.Common.NMS
{
    public class NMSConnectionMonitor : IDisposable
    {
        public bool QuitOk { get; set; }

        private NMSConnectionMonitor()
        {
            QuitOk = true;
        }

        public static NMSConnectionMonitor MonitorConnection(IConnection connection, CancellationTokenSource cts)
        {
            NMSConnectionMonitor monitor = new NMSConnectionMonitor();
            monitor.MonitorConnectionLocal(connection, cts);
            return monitor;
        }

        private void MonitorConnectionLocal(IConnection connection, CancellationTokenSource cts)
        {
            connection.ExceptionListener += (e) =>
            {
                Trace.TraceError("Connection error: {0}", e);
                QuitOk = false;
                cts.Cancel();
            };
        }

        public void Dispose()
        {
        }
    }
}
