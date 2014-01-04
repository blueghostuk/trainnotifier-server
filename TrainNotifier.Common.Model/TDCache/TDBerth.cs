﻿using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.SmartExtract;

namespace TrainNotifier.Common.Model.TDCache
{
    [DataContract]
    public class TDBerth
    {
        [DataMember]
        public string AreaId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime FirstSeen { get; set; }

        [DataMember]
        public DateTime? Exited { get; set; }

        [DataMember]
        public TDElement TDElement { get; set; }

        public TDBerth(string areadId, string name, TDElement element)
        {
            AreaId = areadId;
            Name = name;
            TDElement = element;

            FirstSeen = DateTime.UtcNow;
        }

        public bool IsEntry()
        {
            return Name.Equals("STIN", StringComparison.CurrentCultureIgnoreCase);
        }

        public bool IsExit()
        {
            return Name.Equals("COUT", StringComparison.CurrentCultureIgnoreCase);
        }

    }
}
