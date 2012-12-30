using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainNotifier.ScheduleLibrary;

namespace TrainNotifier.Schedule.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ScheduleService.DownloadSchedule("");
        }
    }
}
