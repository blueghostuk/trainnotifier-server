using Apache.NMS;
using System;
using System.Diagnostics;
using System.Threading;

namespace NetworkRailDownloader.Downloader
{
    public class NMSConnectionMonitor : IDisposable
    {
        private static readonly TimeSpan _defaultTs = TimeSpan.FromMinutes(1);

        private DateTime? _lastMsgRecd;
        private Timer _timer;

        public DateTime? LastMsgRecd
        {
            get { return _lastMsgRecd; }
        }

        private NMSConnectionMonitor() { }

        public static NMSConnectionMonitor MonitorConnection(IConnection connection, IMessageConsumer consumer,
            AutoResetEvent resetEvent, TimeSpan? timeout = null)
        {
            NMSConnectionMonitor monitor = new NMSConnectionMonitor();
            monitor.MonitorConnection(connection, consumer, resetEvent, timeout ?? _defaultTs);
            return monitor;
        }

        private void MonitorConnection(IConnection connection, IMessageConsumer consumer, AutoResetEvent resetEvent, TimeSpan timeout)
        {
            consumer.Listener += (m) =>
            {
                _lastMsgRecd = DateTime.UtcNow;
            };

            connection.ExceptionListener += (e) =>
            {
                Trace.TraceError("Connection error: {0}", e);
                consumer.Close();
                connection.Close();
                resetEvent.Set();
            };

            _timer = new Timer((state) =>
            {
                if (!connection.IsStarted ||
                    !_lastMsgRecd.HasValue ||
                    _lastMsgRecd.Value >= (DateTime.UtcNow.Add(timeout)))
                {
                    consumer.Close();
                    connection.Close();
                    resetEvent.Set();
                }
            }, null, timeout, timeout);
        }

        public void Dispose()
        {
            if (_timer != null)
                _timer.Dispose();
        }
    }
}
