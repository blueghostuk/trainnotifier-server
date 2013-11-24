using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model.PPM
{
    public static class PPMJsonMapper
    {
        public static RtppmData ParsePPMData(dynamic data)
        {
            var ppmData = new RtppmData();
            ppmData.Timestamp = UnixTsToDateTime((double)data.snapshotTStamp);

            foreach (var natSector in data.NationalPage.Sector)
            {
                ppmData.Sectors.Add(ParsePPMNationalRecord(natSector));
            }

            ppmData.NationalPPM = ParsePPMOperatorRecord(data.NationalPage.NationalPPM);

            foreach (var opSector in data.OperatorPage)
            {
                PPMRecord record = ParsePPMOperatorRecord(opSector.Operator);
                try
                {
                    if (opSector.OprServiceGrp != null)
                    {
                        if (opSector.OprServiceGrp is JArray)
                        {
                            foreach (var opServiceGroup in opSector.OprServiceGrp)
                            {
                                record.ServiceGroups.Add(ParsePPMServiceGroupRecord(opServiceGroup));
                            }
                        }
                        else
                        {
                            record.ServiceGroups.Add(ParsePPMServiceGroupRecord(opSector.OprServiceGrp));
                        }
                    }
                }
                catch { }
                ppmData.Operators.Add(record);
            }

            return ppmData;
        }

        public static PPMRecord ParsePPMOperatorRecord(dynamic sector)
        {
            var record = new PPMRecord(false);
            record.Code = sector.code;
            record.Total = DynamicToShort(sector.Total);
            record.OnTime = DynamicToShort(sector.OnTime);
            record.Late = DynamicToShort(sector.Late);
            record.CancelVeryLate = DynamicToShort(sector.CancelVeryLate);
            record.Trend = DynamicToTrend(sector.RollingPPM.trendInd);
            return record;
        }

        public static PPMRecord ParsePPMServiceGroupRecord(dynamic sector)
        {
            var record = new PPMRecord(true);
            record.Code = sector.sectorCode;
            record.Total = DynamicToShort(sector.Total);
            record.OnTime = DynamicToShort(sector.OnTime);
            record.Late = DynamicToShort(sector.Late);
            record.CancelVeryLate = DynamicToShort(sector.CancelVeryLate);
            record.Trend = DynamicToTrend(sector.RollingPPM.trendInd);
            record.Name = sector.name;
            return record;
        }

        public static PPMRecord ParsePPMNationalRecord(dynamic sector)
        {
            var record = new PPMRecord(false);
            record.Code = sector.sectorCode;
            record.Total = DynamicToShort(sector.SectorPPM.Total);
            record.OnTime = DynamicToShort(sector.SectorPPM.OnTime);
            record.Late = DynamicToShort(sector.SectorPPM.Late);
            record.CancelVeryLate = DynamicToShort(sector.SectorPPM.CancelVeryLate);
            record.Trend = DynamicToTrend(sector.SectorPPM.RollingPPM.trendInd);
            return record;
        }

        private static PpmTrendIndicator DynamicToTrend(dynamic value)
        {
            string val = DynamicValueToString(value);
            switch (val)
            {
                case "-":
                    return PpmTrendIndicator.Negative;
                case "+":
                    return PpmTrendIndicator.Positive;
                default:
                    return PpmTrendIndicator.Equal;
            }
        }

        private static short DynamicToShort(dynamic value)
        {
            return Convert.ToInt16(DynamicValueToString(value));
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

            return string.Empty;
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime UnixTsToDateTime(double timeStamp)
        {
            return _epoch.AddMilliseconds(timeStamp);
        }
    }
}
