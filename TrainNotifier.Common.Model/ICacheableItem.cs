using System;

namespace TrainNotifier.Common.Model
{
    public interface ICacheableItem
    {
        TimeSpan? CacheAge { get; set; }
    }
}
