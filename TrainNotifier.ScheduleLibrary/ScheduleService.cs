using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using TrainNotifier.Common;
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

        public static ScheduleTrain ParseJson(dynamic s)
        {
            return new ScheduleTrain
            {
                TrainUid = s.CIF_train_uid,
                //StartDate = DateTime.Parse(s.schedule_start_date),
                //EndDate = string.IsNullOrEmpty(s.schedule_end_date) ? default(DateTime?) : DateTime.Parse(s.schedule_end_date),
                //AtocCode = new AtocCode { Code = s.atoc_code },
                //Status = s.train_status.Equals("P") ? ScheduleStatus.Permanent : s.train_status.Equals("
            };
        }
    }
}
