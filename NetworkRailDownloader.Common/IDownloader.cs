using System;

namespace TrainNotifier.Common
{
    public interface INMSConnector
    {
        event EventHandler<FeedEvent> TrainDataRecieved;

        void SubscribeToFeeds();

        void Quit();
    }

    public sealed class FeedEvent : EventArgs
    {
        public Feed FeedSource { get; private set; }
        public string Data { get; private set; }

        public FeedEvent(Feed source, string data)
        {
            FeedSource = source;
            Data = data;
        }
    }

    public enum Feed
    {
        TrainMovement,
        TrainDescriber,
        VSTP,
        RtPPM
    }

    public enum ScheduleType
    {
        Full,
        DailyUpdate
    }

    public static class TocHelper
    {
        public static string ScheduleTypeToString(this ScheduleType s)
        {
            switch (s)
            {
                case ScheduleType.DailyUpdate:
                    return "UPDATE";
                default:
                //case ScheduleType.Full:
                    return "FULL";
            }
        }

        public static string ScheduleTypeToDay(this ScheduleType s, DayOfWeek? d = null)
        {
            switch (s)
            {
                case ScheduleType.DailyUpdate:
                    const string initial = "toc-update-";
                    switch (d.Value)
                    {
                        case DayOfWeek.Monday:
                            return initial + "mon";
                        case DayOfWeek.Tuesday:
                            return initial + "tue";
                        case DayOfWeek.Wednesday:
                            return initial + "wed";
                        case DayOfWeek.Thursday:
                            return initial + "thu";
                        case DayOfWeek.Friday:
                            return initial + "fri";
                        case DayOfWeek.Saturday:
                            return initial + "sat";
                        case DayOfWeek.Sunday:
                            return initial + "sun";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    //case ScheduleType.Full:
                    return "toc-full";
            }
        }

        public static string TocToString(this Toc t)
        {
            switch (t)
            {
                case Toc.ArrivaTrainsWales:
                    return "HL";
                case Toc.C2C:
                    return "HT";
                case Toc.CrossCountry:
                    return "EH";
                case Toc.DevonAndCornwall:
                    return "EN";
                case Toc.EastMidlandsTrains:
                    return "EM";
                case Toc.Eurostar:
                    return "GA";
                case Toc.FfestiniogRailway:
                    return "XJ";
                case Toc.FirstCapitalConnect:
                    return "EG";
                case Toc.FirstGreatWestern:
                    return "EF";
                case Toc.FirstHullTrains:
                    return "PF";
                case Toc.FirstScotrail:
                    return "HA";
                case Toc.FirstTranspennineExpress:
                    return "EA";
                case Toc.GatwickExpress:
                    return "HV";
                case Toc.GrandCentral:
                    return "EC";
                case Toc.HeathrowConnect:
                    return "EE";
                case Toc.HeathrowExpress:
                    return "HM";
                case Toc.IslandLines:
                    return "HZ";
                case Toc.LondonMidland:
                    return "EJ";
                case Toc.LondonOverground:
                    return "EK";
                case Toc.LULBakerlooLine:
                    return "XC";
                case Toc.LULDistrictLineRichmond:
                    return "XE";
                case Toc.LULDistrictLineWimbledon:
                    return "XB";
                case Toc.Merseyrail:
                    return "HE";
                case Toc.NationalExpressEastAnglia:
                    return "EB";
                case Toc.NationalExpressEastCoast:
                    return "HB";
                case Toc.Nexus:
                    return "PG";
                case Toc.NorthYorkshireMoorsRailway:
                    return "PR";
                case Toc.NorthernRail:
                    return "ED";
                case Toc.Southeastern:
                    return "HU";
                case Toc.Southern:
                    return "HW";
                case Toc.StagecoachSouthWesternTrains:
                    return "HY";
                case Toc.Chiltern:
                    return "HO";
                case Toc.VirginWestCoast:
                    return "HF";
                case Toc.WestCoastRailway:
                    return "PA";
                case Toc.WrexhamAndShropshire:
                    return "EI";
                default:
                //case Toc.All:
                    return "ALL";
            }
        }
    }

    public enum Toc
    {
        All,
        ArrivaTrainsWales,
        C2C,
        CrossCountry,
        DevonAndCornwall,
        EastMidlandsTrains,
        Eurostar,
        FfestiniogRailway,
        FirstCapitalConnect,
        FirstGreatWestern,
        FirstHullTrains,
        FirstScotrail,
        FirstTranspennineExpress,
        GatwickExpress,
        GrandCentral,
        HeathrowConnect,
        HeathrowExpress,
        IslandLines,
        LondonMidland,
        LondonOverground,
        LULBakerlooLine,
        LULDistrictLineRichmond,
        LULDistrictLineWimbledon,
        Merseyrail,
        NationalExpressEastAnglia,
        NationalExpressEastCoast,
        Nexus,
        NorthYorkshireMoorsRailway,
        NorthernRail,
        Southeastern,
        Southern,
        StagecoachSouthWesternTrains,
        Chiltern,
        VirginWestCoast,
        WestCoastRailway,
        WrexhamAndShropshire
    }

}
