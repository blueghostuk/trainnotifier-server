using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRailDownloader.Console.Model
{
    public sealed class TrainMovement
    {
        public DateTime? Activated { get; set; }
        public string Id { get; set; }
        public string ServiceCode { get; set; }
        public string SchedOriginStanox { get; set; }
        public DateTime? SchedOriginDeparture { get; set; }

        private readonly ICollection<TrainMovementStep> _steps;
        public IEnumerable<TrainMovementStep> Steps { get { return _steps; } }

        public TrainMovement()
        {
            _steps = new HashSet<TrainMovementStep>();
        }

        public void AddTrainMovementStep(TrainMovementStep step)
        {
            _steps.Add(step);
        }

    }
}
