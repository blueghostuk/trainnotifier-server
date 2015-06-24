using System;

namespace TrainNotifier.Common
{
    /// <summary>
    /// Interface for connecting and receiving data from an NMS Feed
    /// </summary>
    public interface INMSConnector
    {
        /// <summary>
        /// Event is raised when data is received
        /// </summary>
        event EventHandler<FeedEvent> TrainDataRecieved;

        /// <summary>
        /// Subscribe to the applicable feeds
        /// </summary>
        void SubscribeToFeeds();

        /// <summary>
        /// quit the connection
        /// </summary>
        void Quit();
    }

    /// <summary>
    /// event raised when data received from an NMS connection
    /// </summary>
    public sealed class FeedEvent : EventArgs
    {
        /// <summary>
        /// the source of the data
        /// </summary>
        public Feed FeedSource { get; private set; }
        /// <summary>
        /// the data content
        /// </summary>
        public string Data { get; private set; }

        public FeedEvent(Feed source, string data)
        {
            FeedSource = source;
            Data = data;
        }
    }

    /// <summary>
    /// the feed source
    /// </summary>
    public enum Feed
    {
        TrainMovement,
        TrainDescriber,
        VSTP,
        RtPPM
    }    

}
