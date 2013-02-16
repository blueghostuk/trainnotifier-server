using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TrainNotifier.Common;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Service;

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

        public static ScheduleTrain ParseJsonTrain(dynamic s, ICollection<TiplocCode> tiplocs)
        {
            var t = new ScheduleTrain();
            t.TrainUid = StringField.ParseDataString(DynamicValueToString(s.CIF_train_uid));
            t.StartDate = DateField.ParseDataString(DynamicValueToString(s.schedule_start_date));
            t.EndDate = NullableDateField.ParseDataString(DynamicValueToString(s.schedule_end_date));
            t.AtocCode = AtocCodeField.ParseDataString(DynamicValueToString(s.atoc_code));
            t.Status = StatusField.ParseDataString(DynamicValueToString(s.train_status));
            t.STPIndicator = STPIndicatorField.ParseDataString(DynamicValueToString(s.CIF_stp_indicator));
            t.Schedule = ScheduleField.ParseDataString(DynamicValueToString(s.schedule_days_runs));
            t.Stops = ParseJsonStops(s.schedule_segment.schedule_location, tiplocs);
            if (t.Stops.Any())
            {
                t.Origin = t.Stops
                    .Where(st => st.Origin)
                    .Select(st => st.Tiploc)
                    .FirstOrDefault();
                t.Destination = t.Stops
                    .Where(st => st.Terminate)
                    .Select(st => st.Tiploc)
                    .FirstOrDefault();
            }

            return t;
        }

        public static IEnumerable<ScheduleStop> ParseJsonStops(IEnumerable<dynamic> stops, ICollection<TiplocCode> tiplocs)
        {
            if (stops == null || !stops.Any())
            {
                return Enumerable.Empty<ScheduleStop>();
            }
            ICollection<ScheduleStop> sStops = new List<ScheduleStop>(stops.Count());
            for (byte i = 0; i < stops.Count(); i++)
            {
                dynamic stop = stops.ElementAt(i);
                StopType st = StopTypeField.ParseDataString(DynamicValueToString(stop.location_type));
                string tiplocCode = DynamicValueToString(stop.tiploc_code);
                TiplocCode tiploc = tiplocs.FirstOrDefault(t => t.Tiploc.Equals(tiplocCode, StringComparison.InvariantCultureIgnoreCase));
                if (tiploc == null)
                {
                    TiplocRepository tr = new TiplocRepository();
                    short tiplocId = tr.InsertTiploc(tiplocCode);
                    tiploc = new TiplocCode
                    {
                        TiplocId = tiplocId,
                        Tiploc = tiplocCode
                    };
                    tiplocs.Add(tiploc);
                    Trace.TraceInformation("Added new Tiploc {0} - ID {1}", tiplocCode, tiplocId);
                }
                sStops.Add(new ScheduleStop
                {
                    StopNumber = i,
                    Tiploc = tiploc,
                    Arrival = stop.arrival != null ? TimeSpanField.ParseDataString(DynamicValueToString(stop.arrival)) : default(TimeSpan?),
                    PublicArrival = stop.public_arrival != null ? TimeSpanField.ParseDataString(DynamicValueToString(stop.public_arrival)) : default(TimeSpan?),
                    Departure = TimeSpanField.ParseDataString(DynamicValueToString(stop.departure)),
                    PublicDeparture = TimeSpanField.ParseDataString(DynamicValueToString(stop.public_departure)),
                    Pass = stop.pass != null ? TimeSpanField.ParseDataString(DynamicValueToString(stop.pass)) : default(TimeSpan?),
                    Line = StringField.ParseDataString(DynamicValueToString(stop.line)),
                    Path = stop.path != null ? StringField.ParseDataString(DynamicValueToString(stop.path)) : null,
                    Platform = StringField.ParseDataString(DynamicValueToString(stop.platform)),
                    EngineeringAllowance = ByteField.ParseDataString(DynamicValueToString(stop.engineering_allowance)),
                    PathingAllowance = ByteField.ParseDataString(DynamicValueToString(stop.pathing_allowance)),
                    PerformanceAllowance = ByteField.ParseDataString(DynamicValueToString(stop.performance_allowance)),
                    Origin = st == StopType.Origin,
                    Intermediate = st == StopType.Intermediate,
                    Terminate = st == StopType.Terminate
                });
            }

            return sStops;
        }

        private static string DynamicValueToString(dynamic value)
        {
            try
            {
                if (value is JValue)
                {
                    return ((JValue)value).Value.ToString();
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
