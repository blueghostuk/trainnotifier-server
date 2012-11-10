using Apache.NMS;
using Apache.NMS.Stomp;
using Apache.NMS.Stomp.Commands;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using TrainNotifier.Common;

namespace TrainNotifier.Common.NMS
{
    public sealed class NMSConnector : IDownloader
    {
        private readonly AutoResetEvent _quitSemaphore = new AutoResetEvent(false);

        public event EventHandler<FeedEvent> FeedDataRecieved;

        public NMSConnector()
        {
            Apache.NMS.Tracer.Trace = new NMSTrace();
        }

        private IConnection GetConnection()
        {
            Trace.TraceInformation("Connecting to: {0}", ConfigurationManager.AppSettings["ActiveMQConnectionString"]);
            return new ConnectionFactory(ConfigurationManager.AppSettings["ActiveMQConnectionString"])
                .CreateConnection(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);
        }

        public void DownloadSchedule(string filePath, Toc toc = Toc.All, ScheduleType schedule = ScheduleType.Full, DayOfWeek? day = null)
        {
            Uri requestUri = new Uri(string.Format("{0}?type={1}&day={2}", (object)ConfigurationManager.AppSettings["ScheduleUri"], (object)string.Format("CIF_{0}_{1}_DAILY", (object)TocHelper.TocToString(toc), (object)TocHelper.ScheduleTypeToString(schedule)), (object)TocHelper.ScheduleTypeToDay(schedule, day)));
            byte[] buffer = new byte[4096];
            WebRequest webRequest = WebRequest.Create(requestUri);
            webRequest.Timeout = (int)TimeSpan.FromMinutes((double)int.Parse(ConfigurationManager.AppSettings["ScheduleTimeoutMins"])).TotalMilliseconds;
            string str = Convert.ToBase64String(Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["Username"] + ":" + ConfigurationManager.AppSettings["Password"]));
            webRequest.Headers[HttpRequestHeader.Authorization] = "Basic " + str;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        int count;
                        do
                        {
                            count = responseStream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, count);
                        }
                        while (count != 0);
                        byte[] bytes = memoryStream.ToArray();
                        System.IO.File.WriteAllBytes(filePath, bytes);
                    }
                }
            }
        }

        public void SubscribeToFeed(Feed feed)
        {
            try
            {
                Subscribe();
            }
            catch (Apache.NMS.NMSException nmsE)
            {
                Trace.TraceError("Exception: {0}", nmsE);
                TraceHelper.FlushLog();
                ResubscribeMechanism(feed);
            }
            catch (RetryException re)
            {
                Trace.TraceError("Exception: {0}", re);
                ResubscribeMechanism(feed);
            }
        }

        private byte _retries = 0;
        private readonly byte MaxRetries = 5;

        private void ResubscribeMechanism(Feed feed)
        {
            if (_retries > MaxRetries)
            {
                Trace.TraceError("Exceeded retry count of {0}. Quitting", MaxRetries);
                throw new RetryException();
            }
            TimeSpan retryTs = TimeSpan.FromSeconds(5 * _retries);
            Trace.TraceError("Retry attempt no {0} in {1}", _retries, retryTs);
            Thread.Sleep(TimeSpan.FromSeconds(5 * _retries));
            _retries++;
            SubscribeToFeed(feed);
        }

        private void Subscribe()
        {
            using (IConnection connection = this.GetConnection())
            {
                connection.AcknowledgementMode = AcknowledgementMode.AutoAcknowledge;
                string clientId = ConfigurationManager.AppSettings["ActiveMQDurableClientId"];
                if (!string.IsNullOrEmpty(clientId))
                {
                    connection.ClientId = clientId;
                }
                using (ISession session = connection.CreateSession())
                {
                    ITopic topic = session.GetTopic("TRAIN_MVT_ALL_TOC");
                    using (IMessageConsumer consumer = CreateConsumer(session, topic))
                    {
                        Trace.TraceInformation("Created consumer to {0}", topic);
                        consumer.Listener += new MessageListener(this.consumer_Listener);
                        connection.Start();
                        using (var cm = NMSConnectionMonitor.MonitorConnection(connection, consumer, this._quitSemaphore))
                        {
                            Trace.TraceInformation("Waiting for quit");
                            this._quitSemaphore.WaitOne();
                            if (!cm.QuitOk)
                            {
                                Trace.TraceError("Connection Monitor did not quit OK. Retrying Connection");
                                TraceHelper.FlushLog();
                                throw new RetryException();
                            }
                        }
                        Trace.TraceInformation("Received Quit signal");
                    }
                }
                Trace.TraceInformation("Closing connection to: {0}", connection);
            }
        }

        private class RetryException : Exception { }

        private IMessageConsumer CreateConsumer(ISession session, ITopic destination)
        {
            string subscriberId = ConfigurationManager.AppSettings["ActiveMQDurableSubscriberId"];
            if (!string.IsNullOrEmpty(subscriberId))
                return session.CreateDurableConsumer(destination, subscriberId, null, false);
            else
                return session.CreateConsumer(destination);
        }

        private void consumer_Listener(IMessage message)
        {
            //message.Acknowledge();
            TextMessage textMessage = message as TextMessage;

            if (textMessage == null || null == this.FeedDataRecieved)
                return;

            Trace.TraceInformation("Recd Msg: {0}", textMessage.NMSTimestamp);

            this.FeedDataRecieved((object)this, new FeedEvent(Feed.TrainMovement, textMessage.Text));
        }

        public void Quit()
        {
            this._quitSemaphore.Set();
        }
    }
}
