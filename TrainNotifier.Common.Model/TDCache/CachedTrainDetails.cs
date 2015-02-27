using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model.TDCache
{
    public sealed class CachedTrainDetails
    {
        public Guid Id { get; set; }
        public string TrainUid { get; set; }
        public DateTime OriginDepartTimestamp { get; set; }

        public override string ToString()
        {
            return string.Format("{0};{1}", TrainUid, OriginDepartTimestamp);
        }
    }
}
