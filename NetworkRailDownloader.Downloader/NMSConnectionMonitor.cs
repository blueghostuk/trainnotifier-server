using Apache.NMS;
using System;
using System.Diagnostics;
using System.Threading;

namespace TrainNotifier.Common.NMS
{
    /// <summary>
    /// Connection monitor on an NSM connection
    /// </summary>
    public sealed class NMSConnectionMonitor : IDisposable
    {
        private DateTime? _lastMsgRecd;
        private readonly Timer _timer;

        /// <summary>
        /// Have we quit OK (i.e. without error)
        /// </summary>
        public bool QuitOk { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection">the connection to monitor for messages</param>
        /// <param name="cts">cancellation token to cancel on error or no data received</param>
        /// <param name="timeout">timeout to wait for messages</param>
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
                        _lastMsgRecd.Value <= (DateTime.UtcNow.Subtract(timeout.Value)))
                    {
                        Trace.TraceInformation("Connection Monitor: Connection Stopped or No data recd for {0}, closing connection", timeout);
                        QuitOk = false;
                        cts.Cancel();
                    }
                }, null, timeout.Value, timeout.Value);
            }
        }

        /// <summary>
        /// Add a message consumer to monitor for messages received
        /// </summary>
        public void AddMessageConsumer(IMessageConsumer consumer)
        {
            consumer.Listener += (m) =>
            {
                _lastMsgRecd = DateTime.UtcNow;
            };
        }

        /// <summary>
        /// dispose of the monitor
        /// </summary>
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }
}
