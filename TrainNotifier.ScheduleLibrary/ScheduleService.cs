using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TrainNotifier.Common;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.ScheduleLibrary
{
    public static class ScheduleService
    {
        public static void DownloadSchedule(string filePath, Toc toc = Toc.All, ScheduleType schedule = ScheduleType.Full, DayOfWeek? day = null)
        {
            Uri requestUri = new Uri(string.Format("{0}?type={1}&day={2}", (object)ConfigurationManager.AppSettings["ScheduleUri"], (object)string.Format("CIF_{0}_{1}_DAILY", (object)TocHelper.TocToString(toc), (object)TocHelper.ScheduleTypeToString(schedule)), (object)TocHelper.ScheduleTypeToDay(schedule, day)));
            byte[] buffer = new byte[4096];
            WebRequest webRequest = WebRequest.Create(requestUri);
            webRequest.Timeout = (int)TimeSpan.FromMinutes((double)int.Parse(ConfigurationManager.AppSettings["ScheduleTimeoutMins"])).TotalMilliseconds;
            string str = Convert.ToBase64String(Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["Username"] + ":" + ConfigurationManager.AppSettings["Password"]));
            webRequest.Headers[HttpRequestHeader.Authorization] = "Basic " + str;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        int count;
                        int counter = 0;
                        do
                        {
                            Trace.WriteLine(string.Format("Downloading bytes {0}", counter * 4096));
                            count = responseStream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, count);
                            counter++;
                        }
                        while (count != 0);
                        byte[] bytes = memoryStream.ToArray();
                        System.IO.File.WriteAllBytes(filePath, bytes);
                    }
                }
            }
        }

        public static ScheduleTrain ParseJsonTrain(dynamic s)
        {
            return new ScheduleTrain
            {
                TrainUid = s.CIF_train_uid.Value,
                StartDate = DateField.ParseDataString(s.schedule_start_date.Value),
                EndDate = NullableDateField.ParseDataString(s.schedule_end_date.Value),
                AtocCode = new AtocCode { Code = s.atoc_code.Value },
                Status = StatusField.ParseDataString(s.train_status.Value),
                STPIndicator = STPIndicatorField.ParseDataString(s.CIF_stp_indicator.Value),
                Schedule = ScheduleField.ParseDataString(s.schedule_days_runs.Value),
                Stops = ParseJsonStops(s.schedule_segment.schedule_location).ToList()
            };
        }

        public static IEnumerable<ScheduleStop> ParseJsonStops(IEnumerable<dynamic> stops)
        {
            if (stops == null || !stops.Any())
            {
                return Enumerable.Empty<ScheduleStop>();
            }
            ICollection<ScheduleStop> sStops = new List<ScheduleStop>(stops.Count());
            for(byte i=0; i < stops.Count(); i++)
            {
                dynamic stop = stops.ElementAt(i);
                sStops.Add(new ScheduleStop
                 {
                     StopNumber = i,
                     TiplocCode = stop.tiploc_code.Value,
                     Arrival = stop.arrival != null ? TimeSpanField.ParseDataString(stop.arrival.Value) : default(TimeSpan?),
                     PublicArrival = stop.public_arrival != null ? TimeSpanField.ParseDataString(stop.public_arrival.Value) : default(TimeSpan?),
                     Departure = TimeSpanField.ParseDataString(stop.departure.Value),
                     PublicDeparture = TimeSpanField.ParseDataString(stop.public_departure.Value),
                     Pass = stop.pass != null ? TimeSpanField.ParseDataString(stop.pass.Value) : default(TimeSpan?),
                     Line = StringField.ParseDataString(stop.line.Value),
                     Path = stop.path != null ? StringField.ParseDataString(stop.path.Value) : null,
                     Platform = StringField.ParseDataString(stop.platform.Value),
                     EngineeringAllowance = ByteField.ParseDataString(stop.engineering_allowance.Value),
                     PathingAllowance = ByteField.ParseDataString(stop.pathing_allowance.Value),
                     PerformanceAllowance = ByteField.ParseDataString(stop.performance_allowance.Value),
                     Origin = stop.location_type.Value.Equals("LO", StringComparison.InvariantCultureIgnoreCase),
                     Intermediate = stop.location_type.Value.Equals("LI", StringComparison.InvariantCultureIgnoreCase),
                     Terminate = stop.location_type.Value.Equals("LT", StringComparison.InvariantCultureIgnoreCase)
                 });
            }

            return sStops;
        }
    }
}
