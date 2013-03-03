using System;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Exceptions
{
    [Serializable]
    public class TiplocNotFoundException : Exception
    {
        public string Code { get; set; }
        public TiplocNotFoundException() { }
        public TiplocNotFoundException(string message) : base(message) { }
        public TiplocNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected TiplocNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
