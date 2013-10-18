using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    [Flags]
    public enum TrainState : byte
    {
        [EnumMember]
        Activated = 1,
        [EnumMember]
        Cancelled = 2,
        [EnumMember]
        Terminated = 4
    }

    [DataContract]
    public enum TrainMovementEventType : byte
    {
        [EnumMember]
        Departure = 1,
        [EnumMember]
        Arrival = 2
    }

    [DataContract]
    public enum TrainMovementVariationStatus : byte
    {
        [EnumMember]
        OnTime = 1,
        [EnumMember]
        Early = 2,
        [EnumMember]
        Late = 3,
        [EnumMember]
        OffRoute = 4
    }
}
