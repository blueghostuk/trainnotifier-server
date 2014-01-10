using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Service
{
    // this will be a readonly view of LiveTrainRepository - it needs to pre-load activations
    // and update the cache if cannot find existing entry by looking up in db - currently commented out in LiveTrainRepository
    // memory should be ConcurrentDictionary

    //class LiveTrainLookupRepository
    //{
    //}
}
