using System;

namespace TrainNotifier.Common.Model.Exceptions
{
    public class TiplocNotFoundException : Exception
    {
        public string Code { get; set; }
        public TiplocNotFoundException() { }
        public TiplocNotFoundException(string message) : base(message) { }
        public TiplocNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
