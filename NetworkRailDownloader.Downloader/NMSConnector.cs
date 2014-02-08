﻿using Apache.NMS;
using Apache.NMS.Stomp;
using Apache.NMS.Stomp.Commands;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrainNotifier.Common;

namespace TrainNotifier.Common.NMS
{
    public sealed class NMSConnector : INMSConnector
    {
        public event EventHandler<FeedEvent> TrainDataRecieved;

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

        public void SubscribeToFeeds()
        {
            try
            {
                Subscribe();
            }
            catch (Apache.NMS.NMSException nmsE)
            {
                Trace.TraceError("Exception: {0}", nmsE);
                TraceHelper.FlushLog();
                ResubscribeMechanism();
            }
            catch (RetryException re)
            {
                Trace.TraceError("Exception: {0}", re);
                ResubscribeMechanism();
            }
            catch (AggregateException ae)
            {
                Trace.TraceError("Exception: {0}", ae);
                ResubscribeMechanism();
            }
        }

        private byte _retries = 0;
        private readonly byte MaxRetries = 5;
    
        private void ResubscribeMechanism()
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
            SubscribeToFeeds();
        }

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private void Subscribe()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            using (IConnection connection = this.GetConnection())
            {
                connection.AcknowledgementMode = AcknowledgementMode.AutoAcknowledge;
                string clientId = ConfigurationManager.AppSettings["ActiveMQDurableClientId"];
                if (!string.IsNullOrEmpty(clientId))
                {
                    connection.ClientId = clientId;
                }
                using (var connectionMonitor = new NMSConnectionMonitor(connection, _cancellationTokenSource, TimeSpan.FromMinutes(3)))
                {
                    connection.Start();

                    Task tmDataTask = Task.Factory.StartNew(() => GetTrainMovementData(connection, _cancellationTokenSource.Token, connectionMonitor), _cancellationTokenSource.Token);
                    Task tdDataTask = Task.Factory.StartNew(() => GetTrainDescriberData(connection, _cancellationTokenSource.Token, connectionMonitor), _cancellationTokenSource.Token);
                    Task vstpDataTask = Task.Factory.StartNew(() => GetVSTPData(connection, _cancellationTokenSource.Token, connectionMonitor), _cancellationTokenSource.Token);
                    Task rtppmTask = Task.Factory.StartNew(() => GetRtPPMData(connection, _cancellationTokenSource.Token, connectionMonitor), _cancellationTokenSource.Token);

                    try
                    {
                        Task.WaitAll(new[] { tmDataTask, tdDataTask, vstpDataTask, rtppmTask }, _cancellationTokenSource.Token);
                        if (!connectionMonitor.QuitOk)
                        {
                            Trace.TraceError("Connection Monitor did not quit OK. Retrying Connection");
                            TraceHelper.FlushLog();
                            throw new RetryException();
                        }
                        _cancellationTokenSource.Cancel();
                        Trace.TraceInformation("Closing connection to: {0}", connection);
                    }
                    catch (OperationCanceledException)
                    {
                        Trace.TraceError("Connection Monitor cancelled");
                        TraceHelper.FlushLog();
                    }
                }
            }
        }

        private void GetRtPPMData(IConnection connection, CancellationToken cancellationToken, NMSConnectionMonitor connectionMonitor)
        {
            string rtppmTopic = ConfigurationManager.AppSettings["RTPPMFeedName"];
            if (!string.IsNullOrEmpty(rtppmTopic))
            {
                using (ISession session = connection.CreateSession())
                {
                    ITopic topic = session.GetTopic(rtppmTopic);
                    OpenAndWaitConsumer(session, topic, "rtppm", connectionMonitor, this.rtppm_Listener, cancellationToken);
                }
            }
        }

        private void GetVSTPData(IConnection connection, CancellationToken ct, NMSConnectionMonitor connectionMonitor)
        {
            string vstpTopic = ConfigurationManager.AppSettings["VSTPFeedName"];
            if (!string.IsNullOrEmpty(vstpTopic))
            {
                using (ISession session = connection.CreateSession())
                {
                    ITopic topic = session.GetTopic(vstpTopic);
                    OpenAndWaitConsumer(session, topic, "vstp", connectionMonitor, this.vstpConsumer_Listener, ct);
                }
            }
        }

        private void GetTrainMovementData(IConnection connection, CancellationToken ct, NMSConnectionMonitor connectionMonitor)
        {
            string trainMovementTopic = ConfigurationManager.AppSettings["TrainMovementName"];
            if (!string.IsNullOrEmpty(trainMovementTopic))
            {
                using (ISession session = connection.CreateSession())
                {
                    ITopic topic = session.GetTopic(trainMovementTopic /*"TRAIN_MVT_ALL_TOC"*/);
                    OpenAndWaitConsumer(session, topic, "tm", connectionMonitor, this.tmConsumer_Listener, ct);
                }
            }
        }

        private void GetTrainDescriberData(IConnection connection, CancellationToken ct, NMSConnectionMonitor connectionMonitor)
        {
            string trainDescriberTopic = ConfigurationManager.AppSettings["TrainDescriberName"];
            if (!string.IsNullOrEmpty(trainDescriberTopic))
            {
                using (ISession session = connection.CreateSession())
                {
                    ITopic topic = session.GetTopic(trainDescriberTopic/*"TD_LNW_WMC_SIG_AREA"*/);
                    OpenAndWaitConsumer(session, topic, "td", connectionMonitor, this.tdConsumer_Listener, ct);
                }
            }
        }

        private void OpenAndWaitConsumer(ISession session, ITopic topic, string appendedText, NMSConnectionMonitor connectionMonitor, MessageListener listener, CancellationToken ct)
        {
            using (IMessageConsumer consumer = CreateConsumer(session, topic, appendedText))
            {
                Trace.TraceInformation("Created consumer to {0}", topic);
                // dont check expiry
                MessageConsumer messageConsumer = consumer as MessageConsumer;
                if (messageConsumer != null)
                {
                    messageConsumer.CheckExpiry = false;
                }

                consumer.Listener += listener;
                connectionMonitor.AddMessageConsumer(consumer);
                ct.WaitHandle.WaitOne();
            }
        }

        private class RetryException : Exception { }

        private IMessageConsumer CreateConsumer(ISession session, ITopic destination, string appendedText = null)
        {
            string subscriberId = ConfigurationManager.AppSettings["ActiveMQDurableSubscriberId"];
            if (!string.IsNullOrEmpty(subscriberId))
                return session.CreateDurableConsumer(destination, string.Concat(subscriberId, appendedText), null, false);
            else
                return session.CreateConsumer(destination);
        }

        private void tmConsumer_Listener(IMessage message)
        {
            string text = ParseData(message);
            if (!string.IsNullOrEmpty(text))
            {
                RaiseDataRecd(Feed.TrainMovement, text);
            }
        }

        private void tdConsumer_Listener(IMessage message)
        {
            string text = ParseData(message);
            if (!string.IsNullOrEmpty(text))
            {
                RaiseDataRecd(Feed.TrainDescriber, text);
            }
        }

        private static readonly bool LogVstp = bool.Parse(ConfigurationManager.AppSettings["VSTPLogging"]);

        private void vstpConsumer_Listener(IMessage message)
        {
            string text = ParseData(message);
            if (!string.IsNullOrEmpty(text))
            {
                RaiseDataRecd(Feed.VSTP, text);
                if (LogVstp)
                {
                    Trace.TraceInformation("VSTP:{0}", text);
                }
            }
        }

        private void rtppm_Listener(IMessage message)
        {
            string text = ParseData(message);
            if (!string.IsNullOrEmpty(text))
            {
                RaiseDataRecd(Feed.RtPPM, text);
            }
        }

        private string ParseData(IMessage message)
        {
            TextMessage textMessage = message as TextMessage;

            if (textMessage == null)
                return null;

            Trace.TraceInformation("[{0}] - Recd Msg for {1}", 
                textMessage.FromDestination,
                textMessage.NMSTimestamp);

            return textMessage.Text;
        }

        private void RaiseDataRecd(Feed source, string text)
        {
            var eh = this.TrainDataRecieved;
            if (null != eh)
                eh(this, new FeedEvent(source, text));
        }

        public void Quit()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}
