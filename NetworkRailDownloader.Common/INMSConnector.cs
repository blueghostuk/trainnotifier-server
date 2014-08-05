using System;

namespace TrainNotifier.Common
{
    public interface INMSConnector
    {
        event EventHandler<FeedEvent> TrainDataRecieved;

        void SubscribeToFeeds();

        void Quit();
    }

    public sealed class FeedEvent : EventArgs
    {
        public Feed FeedSource { get; private set; }
        public string Data { get; private set; }

        public FeedEvent(Feed source, string data)
        {
            FeedSource = source;
            Data = data;
        }
    }

    public enum Feed
    {
        TrainMovement,
        TrainDescriber,
        VSTP,
        RtPPM
    }    

}
