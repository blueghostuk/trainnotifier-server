using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainNotifier.Common.Exceptions;

namespace TrainNotifier.Common.Model.Schedule
{
    public static class ScheduleTrainMapper
    {
        /// <exception cref="TiplocNotFoundException"></exception>
        public static ScheduleTrain ParseJsonTrain(dynamic s, IEnumerable<TiplocCode> tiplocs)
        {
            var t = new ScheduleTrain();
            t.TransactionType = TransactionTypeField.ParseDataString(DynamicValueToString(s.transaction_type));
            t.TrainUid = StringField.ParseDataString(DynamicValueToString(s.CIF_train_uid));
            t.StartDate = DateField.ParseDataString(DynamicValueToString(s.schedule_start_date));
            t.STPIndicator = STPIndicatorField.ParseDataString(DynamicValueToString(s.CIF_stp_indicator));
            switch (t.TransactionType)
            {
                case TransactionType.Create:
                    t.EndDate = NullableDateField.ParseDataString(DynamicValueToString(s.schedule_end_date));
                    t.AtocCode = AtocCodeField.ParseDataString(DynamicValueToString(s.atoc_code));
                    t.Status = StatusField.ParseDataString(DynamicValueToString(s.train_status));
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
                    break;
            }

            return t;
        }

        /// <exception cref="TiplocNotFoundException"></exception>
        public static IEnumerable<ScheduleStop> ParseJsonStops(IEnumerable<dynamic> stops, IEnumerable<TiplocCode> tiplocs)
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
                    throw new TiplocNotFoundException
                    {
                        Code = tiplocCode
                    };
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
