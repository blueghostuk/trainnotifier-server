using Apache.NMS;
using System;
using System.Diagnostics;
using System.Threading;

namespace TrainNotifier.Common.NMS
{
    public sealed class NMSConnectionMonitor : IDisposable
    {
        private DateTime? _lastMsgRecd;
        private readonly Timer _timer;

        public bool QuitOk { get; set; }

        public NMSConnectionMonitor(IConnection connection, CancellationTokenSource cts, TimeSpan? timeout = null)
        {
            QuitOk = true;
            connection.ExceptionListener += (e) =>
            {
                Trace.TraceError("Connection error: {0}", e);
                QuitOk = false;
                cts.Cancel();
            };

            if (timeout.HasValue)
            {
                _timer = new Timer((state) =>
                {
                    if (!connection.IsStarted ||
                        !_lastMsgRecd.HasValue ||
                        _lastMsgRecd.Value >= (DateTime.UtcNow.Add(timeout.Value)))
                    {
                        Trace.TraceInformation("Connection Monitor: Connection Stopped or No data recd for {0}, closing connection", timeout);
                        QuitOk = false;
                        cts.Cancel();
                    }
                }, null, timeout.Value, timeout.Value);
            }
        }

        public void AddMessageConsumer(IMessageConsumer consumer)
        {
            consumer.Listener += (m) =>
            {
                _lastMsgRecd = DateTime.UtcNow;
            };
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }
}
