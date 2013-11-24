
namespace TrainNotifier.Common.Model
{
    public class CommandResponse<T>
    {
        public string Command { get; set; }
        public string Args { get; set; }

        public T Response { get; set; }
    }
}
