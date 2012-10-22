﻿// Type: NetworkRailDownloader.Downloader.NMSConnector
// Assembly: NetworkRailDownloader.Downloader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: E:\Documents\GitHub\networkrail-downloader - Copy\NetworkRailDownloader\bin\Debug-Local\NetworkRailDownloader.Downloader.dll

using Apache.NMS;
using Apache.NMS.Stomp;
using Apache.NMS.Stomp.Commands;
using NetworkRailDownloader.Common;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace NetworkRailDownloader.Downloader
{
    public sealed class NMSConnector : IDownloader
    {
        protected AutoResetEvent _quitSemaphore = new AutoResetEvent(false);

        public event EventHandler<FeedEvent> FeedDataRecieved;

        private IConnection GetConnection()
        {
            return new ConnectionFactory(ConfigurationManager.AppSettings["ActiveMQConnectionString"]).CreateConnection(ConfigurationManager.AppSettings["Username"], 
                ConfigurationManager.AppSettings["Password"]);
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
                Subscribe();
            }
        }

        private void Subscribe()
        {
            using (IConnection connection = this.GetConnection())
            {
                using (ISession session = connection.CreateSession())
                {
                    ITopic topic = session.GetTopic("TRAIN_MVT_ALL_TOC");
                    using (IMessageConsumer consumer = session.CreateConsumer((IDestination)topic))
                    {
                        connection.Start();
                        consumer.Listener += new MessageListener(this.consumer_Listener);
                        this._quitSemaphore.WaitOne();
                    }
                }
            }
        }

        private void consumer_Listener(IMessage message)
        {
            message.Acknowledge();
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
