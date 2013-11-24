using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public enum TransactionType
    {
        [EnumMember()]
        Create,
        [EnumMember()]
        Delete
    }

    public sealed class TransactionTypeField : EnumField<TransactionType>
    {
        public static readonly TransactionTypeField Default = new TransactionTypeField();

        public static TransactionType ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public TransactionTypeField()
            : base(0, new Dictionary<string, TransactionType>
            {
                { "Create", TransactionType.Create },
                { "Delete", TransactionType.Delete },
            }, defaultValue : TransactionType.Create) { }
    }

    public enum BankHolidayRunning
    {
        Runs,
        DoesNotRunOnBankHolidayMondays,
        [Obsolete]
        DoesNotRunOnEdinburghHolidays,
        DoesNotRunOnGlasgowHolidays
    }

    public sealed class BankHolidayRunningField : EnumField<BankHolidayRunning?>
    {
        public static readonly BankHolidayRunningField Default = new BankHolidayRunningField();

        public static BankHolidayRunning? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public BankHolidayRunningField()
            : base(1, new Dictionary<string, BankHolidayRunning?>
            {
                { "X", BankHolidayRunning.DoesNotRunOnBankHolidayMondays },
                //{ "E", BankHolidayRunning.DoesNotRunOnEdinburghHolidays },
                { "G", BankHolidayRunning.DoesNotRunOnGlasgowHolidays },
            }, defaultValue: BankHolidayRunning.Runs) { }
    }

    [DataContract]
    public enum Status : byte
    {
        [EnumMember()]
        Bus = 1,
        [EnumMember()]
        Freight,
        [EnumMember()]
        PassengerAndParcels,
        [EnumMember()]
        Ship,
        [EnumMember()]
        Trip,
        [EnumMember()]
        STPPassengerAndParcels,
        [EnumMember()]
        STPFreight,
        [EnumMember()]
        STPTrip,
        [EnumMember()]
        STPShip,
        [EnumMember()]
        STPBus
    }

    public sealed class StatusField : EnumField<Status?>
    {
        public static readonly StatusField Default = new StatusField();

        public static Status? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public StatusField()
            : base(1, new Dictionary<string, Status?>
            {
                { "B", Status.Bus },
                { "F", Status.Freight },
                { "P", Status.PassengerAndParcels },
                { "S", Status.Ship },
                { "T", Status.Trip },
                { "1", Status.STPPassengerAndParcels },
                { "2", Status.STPFreight },
                { "3", Status.STPTrip },
                { "4", Status.STPShip },
                { "5", Status.STPBus }
            }) { }
    }

    [DataContract]
    public enum TrainCategory : byte
    {
        [EnumMember()]
        LondonUndergroundMetro = 1,
        [EnumMember()]
        UnadvertisedPassenger,
        [EnumMember()]
        OrdinaryPassenger,
        [EnumMember()]
        Staff,
        [EnumMember()]
        Mixed,
        [EnumMember()]
        ChannelTunnel,
        [EnumMember()]
        SleeperEurope,
        [EnumMember()]
        International,
        [EnumMember()]
        Motorail,
        [EnumMember()]
        UnadvertisedExpress,
        [EnumMember()]
        ExpressPassenger,
        [EnumMember()]
        SleeperDomestic,
        [EnumMember()]
        BusReplacementEngineering,
        [EnumMember()]
        BusWTT,
        [EnumMember()]
        ECS,
        [EnumMember()]
        ECSLondonUndergroundMetro,
        [EnumMember()]
        ECSStaff,
        [EnumMember()]
        Postal,
        [EnumMember()]
        PostOfficeParcels,
        [EnumMember()]
        Parcels,
        [EnumMember()]
        EmptyNPCCS,
        [EnumMember()]
        Departmental,
        [EnumMember()]
        CivilEngineer,
        [EnumMember()]
        MechanicalAndElectricalEngineer,
        [EnumMember()]
        Stores,
        [EnumMember()]
        Test,
        [EnumMember()]
        SignalAndTelecommsEngineer,
        [EnumMember()]
        LocoAndBrake,
        [EnumMember()]
        LightLoco,
        [EnumMember()]
        RfDAutomotiveComponents,
        [EnumMember()]
        RfDAutomotiveVehicles,
        [EnumMember()]
        RfDEdible,
        [EnumMember()]
        RfDIndustrialMinerals,
        [EnumMember()]
        RfDChemicals,
        [EnumMember()]
        RfDBuildingMaterials,
        [EnumMember()]
        RfDGeneralMerch,
        [EnumMember()]
        RfDEuropean,
        [EnumMember()]
        RfDFreightlinerContract,
        [EnumMember()]
        RfDFreightlinerOther,
        [EnumMember()]
        CoalDistributive,
        [EnumMember()]
        CoalElectrical,
        [EnumMember()]
        CoalAndNuclear,
        [EnumMember()]
        Metals,
        [EnumMember()]
        Aggreggates,
        [EnumMember()]
        DomesticAndIndustrialWaste,
        [EnumMember()]
        BuildingMaterials,
        [EnumMember()]
        Petroleum,
        [EnumMember()]
        RfDEuroChannelTunnelMixed,
        [EnumMember()]
        RfDEuroChannelTunnelIntermodal,
        [EnumMember()]
        RfDEuroChannelTunnelAutomotive,
        [EnumMember()]
        RfDEuroChannelTunnelContractServices,
        [EnumMember()]
        RfDEuroChannelTunnelHaulmark,
        [EnumMember()]
        RfDEuroChannelTunnelJointVenture
    }

    public sealed class TrainCategoryField : EnumField<TrainCategory?>
    {
        public static readonly TrainCategoryField Default = new TrainCategoryField();

        public static TrainCategory? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public TrainCategoryField()
            : base(2, new Dictionary<string, TrainCategory?>
            {
                { "OL", TrainCategory.LondonUndergroundMetro },
                { "OU", TrainCategory.UnadvertisedPassenger },
                { "OO", TrainCategory.OrdinaryPassenger },
                { "OS", TrainCategory.Staff },
                { "OW", TrainCategory.Mixed },
                { "XC", TrainCategory.ChannelTunnel },
                { "XD", TrainCategory.SleeperEurope },
                { "XI", TrainCategory.International },
                { "XR", TrainCategory.Motorail },
                { "XU", TrainCategory.UnadvertisedExpress },
                { "XX", TrainCategory.ExpressPassenger },
                { "XZ", TrainCategory.SleeperDomestic },
                { "BR", TrainCategory.BusReplacementEngineering },
                { "BS", TrainCategory.BusWTT },
                { "EE", TrainCategory.ECS },
                { "EL", TrainCategory.ECSLondonUndergroundMetro },
                { "ES", TrainCategory.ECSStaff },
                { "JJ", TrainCategory.Postal },
                { "PM", TrainCategory.PostOfficeParcels },
                { "PP", TrainCategory.Parcels },
                { "PV", TrainCategory.EmptyNPCCS },
                { "DD", TrainCategory.Departmental },
                { "DH", TrainCategory.CivilEngineer },
                { "DI", TrainCategory.MechanicalAndElectricalEngineer },
                { "DQ", TrainCategory.Stores },
                { "DT", TrainCategory.Test },
                { "DY", TrainCategory.SignalAndTelecommsEngineer },
                { "ZB", TrainCategory.LocoAndBrake },
                { "ZZ", TrainCategory.LightLoco },
                { "J2", TrainCategory.RfDAutomotiveComponents },
                { "H2", TrainCategory.RfDAutomotiveVehicles },
                { "J3", TrainCategory.RfDEdible },
                { "J4", TrainCategory.RfDIndustrialMinerals },
                { "J5", TrainCategory.RfDChemicals },
                { "J6", TrainCategory.RfDBuildingMaterials },
                { "J8", TrainCategory.RfDGeneralMerch },
                { "H8", TrainCategory.RfDEuropean },
                { "J9", TrainCategory.RfDFreightlinerContract },
                { "H9", TrainCategory.RfDFreightlinerOther },
                { "A0", TrainCategory.CoalDistributive },
                { "E0", TrainCategory.CoalElectrical },
                { "B0", TrainCategory.CoalAndNuclear },
                { "B1", TrainCategory.Metals },
                { "B4", TrainCategory.Aggreggates },
                { "B5", TrainCategory.DomesticAndIndustrialWaste },
                { "B6", TrainCategory.BuildingMaterials },
                { "B7", TrainCategory.Petroleum },
                { "H0", TrainCategory.RfDEuroChannelTunnelMixed },
                { "H1", TrainCategory.RfDEuroChannelTunnelIntermodal },
                { "H3", TrainCategory.RfDEuroChannelTunnelAutomotive },
                { "H4", TrainCategory.RfDEuroChannelTunnelContractServices },
                { "H5", TrainCategory.RfDEuroChannelTunnelHaulmark },
                { "H6", TrainCategory.RfDEuroChannelTunnelJointVenture }
            }) { }
    }

    [DataContract]
    public enum PowerType : byte
    {
        [EnumMember]
        Diesel = 1,
        [EnumMember]
        DieselElectricMultipleUnit = 2,
        [EnumMember]
        DieselMechanicalMultipleUnit = 3,
        [EnumMember]
        Electric = 4,
        [EnumMember]
        ElectroDiesel = 5,
        [EnumMember]
        EML = 6,
        [EnumMember]
        ElectricMultipleUnit = 7,
        [EnumMember]
        ElectricParcelsUnit = 8,
        [EnumMember]
        HighSpeedTrain = 9,
        [EnumMember]
        DieselShuntingLocomotive = 10
    }

    public sealed class PowerTypeField : EnumField<PowerType?>
    {
        public static readonly PowerTypeField Default = new PowerTypeField();

        public static PowerType? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public PowerTypeField()
            : base(3, new Dictionary<string, PowerType?>
            {
                { "D", PowerType.Diesel },
                { "DEM", PowerType.DieselElectricMultipleUnit },
                { "DMU", PowerType.DieselMechanicalMultipleUnit },
                { "E", PowerType.Electric },
                { "ED", PowerType.ElectroDiesel },
                { "EML", PowerType.EML },
                { "EMU", PowerType.ElectricMultipleUnit },
                { "EPU", PowerType.ElectricParcelsUnit },
                { "HST", PowerType.HighSpeedTrain },
                { "LDS", PowerType.DieselShuntingLocomotive },
            }) { }
    }

    public enum OperatingCharacteristics
    {
        VacuumBraked,
        TimedAt100Mph,
        DOO,
        ConveysMark4Coaches,
        GuardRequired,
        TimedAt110Mph,
        PushPullTrain,
        RunsAsRequired,
        AirConditionedWithPA,
        SteamHeated,
        RunsToTerminalAsRequired,
        MayConveySB1CGauge
    }

    public sealed class OperatingCharacteristicsField : EnumField<OperatingCharacteristics?>
    {
        public static readonly OperatingCharacteristicsField Default = new OperatingCharacteristicsField();

        public static OperatingCharacteristics? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public OperatingCharacteristicsField()
            : base(1, new Dictionary<string, OperatingCharacteristics?>
            {
                { "B", OperatingCharacteristics.VacuumBraked },
                { "C", OperatingCharacteristics.TimedAt100Mph },
                { "D", OperatingCharacteristics.DOO },
                { "E", OperatingCharacteristics.ConveysMark4Coaches },
                { "G", OperatingCharacteristics.GuardRequired },
                { "M", OperatingCharacteristics.TimedAt110Mph },
                { "P", OperatingCharacteristics.PushPullTrain },
                { "Q", OperatingCharacteristics.RunsAsRequired },
                { "R", OperatingCharacteristics.AirConditionedWithPA },
                { "S", OperatingCharacteristics.SteamHeated },
                { "Y", OperatingCharacteristics.RunsToTerminalAsRequired },
                { "Z", OperatingCharacteristics.MayConveySB1CGauge },
            }) { }
    }

    public enum TrainClass
    {
        FirstAndStandard,
        StandardOnly,
        FirstOnly
    }

    public class TrainClassField : EnumField<TrainClass?>
    {
        public static readonly TrainClassField Default = new TrainClassField();

        public static TrainClass? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public TrainClassField()
            : base(1, new Dictionary<string, TrainClass?>{
                { "B", TrainClass.FirstAndStandard },
                { "S", TrainClass.StandardOnly },
                { "F", TrainClass.FirstOnly }
            }, defaultValue: TrainClass.FirstAndStandard) { }
    }

    public sealed class SleepersField : EnumField<TrainClass?>
    {
        public static readonly SleepersField Default = new SleepersField();

        public static TrainClass? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public SleepersField()
            : base(1, new Dictionary<string, TrainClass?>{
                { "B", TrainClass.FirstAndStandard },
                { "S", TrainClass.StandardOnly },
                { "F", TrainClass.FirstOnly }
            }) { }
    }

    public enum Reservations
    {
        SeatCompulsory,
        BicyclesEssential,
        SeatRecommended,
        SeatPossibleFromAnyStation
    }

    public sealed class ReservationsField : EnumField<Reservations?>
    {
        public static readonly ReservationsField Default = new ReservationsField();

        public static Reservations? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public ReservationsField()
            : base(1, new Dictionary<string, Reservations?>{
                { "A", Reservations.SeatCompulsory },
                { "E", Reservations.BicyclesEssential },
                { "R", Reservations.SeatRecommended },
                { "S", Reservations.SeatPossibleFromAnyStation }
            }) { }
    }

    public enum Catering
    {
        BuffetService,
        RestaurantForFirstClass,
        HotFootAvailable,
        MealIncForFirstClass,
        WheelchairOnlyReservations,
        Restaurant,
        TrolleyService
    }

    public sealed class CateringField : EnumField<Catering?>
    {
        public static readonly CateringField Default = new CateringField();

        public static Catering? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public CateringField()
            : base(1, new Dictionary<string, Catering?>{
                { "C", Catering.BuffetService },
                { "F", Catering.RestaurantForFirstClass },
                { "H", Catering.HotFootAvailable },
                { "M", Catering.MealIncForFirstClass },
                { "P", Catering.WheelchairOnlyReservations },
                { "R", Catering.Restaurant },
                { "T", Catering.TrolleyService }
            }) { }
    }

    public enum ServiceBranding
    {
        Eurostar,
        Alphaline
    }

    public sealed class ServiceBrandingField : EnumField<ServiceBranding?>
    {
        public static readonly ServiceBrandingField Default = new ServiceBrandingField();

        public static ServiceBranding? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public ServiceBrandingField()
            : base(1, new Dictionary<string, ServiceBranding?>{
                { "E", ServiceBranding.Eurostar },
                { "U", ServiceBranding.Alphaline }
            }) { }
    }


    [DataContract]
    public sealed class Schedule
    {
        [DataMember]
        public bool Monday { get; set; }
        [DataMember]
        public bool Tuesday { get; set; }
        [DataMember]
        public bool Wednesday { get; set; }
        [DataMember]
        public bool Thursday { get; set; }
        [DataMember]
        public bool Friday { get; set; }
        [DataMember]
        public bool Saturday { get; set; }
        [DataMember]
        public bool Sunday { get; set; }
        [DataMember]
        public bool BankHoliday { get; set; }

        public bool RunsOnDay(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return Monday;
                case DayOfWeek.Tuesday:
                    return Tuesday;
                case DayOfWeek.Wednesday:
                    return Wednesday;
                case DayOfWeek.Thursday:
                    return Thursday;
                case DayOfWeek.Friday:
                    return Friday;
                case DayOfWeek.Saturday:
                    return Saturday;
                case DayOfWeek.Sunday:
                    return Sunday;
                default:
                    return false;
            }
        }

    }

    public sealed class ScheduleField : RecordField<Schedule>
    {
        public static readonly ScheduleField Default = new ScheduleField();

        public static Schedule ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public ScheduleField()
            : base(7) { }

        public override void ParseString(string data)
        {
            Value = new Schedule
            {
                Monday = DayOfWeekField.Monday.ParseDataString(data[0].ToString()).HasValue,
                Tuesday = DayOfWeekField.Tuesday.ParseDataString(data[1].ToString()).HasValue,
                Wednesday = DayOfWeekField.Wednesday.ParseDataString(data[2].ToString()).HasValue,
                Thursday = DayOfWeekField.Thursday.ParseDataString(data[3].ToString()).HasValue,
                Friday = DayOfWeekField.Friday.ParseDataString(data[4].ToString()).HasValue,
                Saturday = DayOfWeekField.Saturday.ParseDataString(data[5].ToString()).HasValue,
                Sunday = DayOfWeekField.Sunday.ParseDataString(data[6].ToString()).HasValue
            };
        }

    }

    [DataContract]
    public enum STPIndicator : byte
    {
        [EnumMember()]
        Cancellation = 1,
        [EnumMember()]
        STP,
        [EnumMember()]
        Overlay,
        [EnumMember()]
        Permanent
    }

    public sealed class TrainAssociationDateField : EnumField<TrainNotifier.Common.Model.AssociationDate>
    {
        public static readonly TrainAssociationDateField Default = new TrainAssociationDateField();

        public static TrainNotifier.Common.Model.AssociationDate ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public TrainAssociationDateField()
            : base(2, new Dictionary<string, TrainNotifier.Common.Model.AssociationDate>{
                { "S", TrainNotifier.Common.Model.AssociationDate.SameDay },
                { "P", TrainNotifier.Common.Model.AssociationDate.PreviousDay },
                { "N", TrainNotifier.Common.Model.AssociationDate.NextDay }
            }, defaultValue: TrainNotifier.Common.Model.AssociationDate.SameDay) { }
    }

    public sealed class TrainAssociationTypeField : EnumField<TrainNotifier.Common.Model.AssociationType>
    {
        public static readonly TrainAssociationTypeField Default = new TrainAssociationTypeField();

        public static TrainNotifier.Common.Model.AssociationType ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public TrainAssociationTypeField()
            : base(2, new Dictionary<string, TrainNotifier.Common.Model.AssociationType>{
                { "NP", TrainNotifier.Common.Model.AssociationType.NextTrain },
                { "JJ", TrainNotifier.Common.Model.AssociationType.Join },
                { "VV", TrainNotifier.Common.Model.AssociationType.Split }
            }, defaultValue: TrainNotifier.Common.Model.AssociationType.NextTrain) { }
    }

    public sealed class STPIndicatorField : EnumField<STPIndicator?>
    {
        public static readonly STPIndicatorField Default = new STPIndicatorField();

        public static STPIndicator? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public STPIndicatorField()
            : base(1, new Dictionary<string, STPIndicator?>{
                { "C", STPIndicator.Cancellation },
                { "N", STPIndicator.STP },
                { "O", STPIndicator.Overlay },
                { "P", STPIndicator.Permanent }
            }) { }
    }

    /// <remarks>
    /// Also used for Performance Allowance
    /// </remarks>
    public sealed class EngineeringAllowanceField : RecordField<TimeSpan?>
    {
        public static readonly EngineeringAllowanceField Default = new EngineeringAllowanceField();

        public static TimeSpan? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public EngineeringAllowanceField()
            : base(2)
        { }

        public override void ParseString(string data)
        {
            data = data.Trim();
            if (data.Length == 1 && data[0] == Constants.HalfMinute)
            {
                Value = Constants.HalfMinuteAmount;
            }
            else if (data.Length >= 1)
            {
                Value = TimeSpan.FromMinutes(double.Parse(data[0].ToString()));
                if (data.Length == 2)
                {
                    if (data[1] == Constants.HalfMinute)
                    {
                        Value = Value.Value.Add(Constants.HalfMinuteAmount);
                    }
                }
            }
            else
            {
                Value = null;
            }
        }
    }

    public sealed class PathingTimeField : RecordField<TimeSpan?>
    {
        public static readonly PathingTimeField Default = new PathingTimeField();

        public static TimeSpan? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public PathingTimeField()
            : base(2)
        { }

        public override void ParseString(string data)
        {
            data = data.Trim();
            if (data.Length == 1 && data[0] == Constants.HalfMinute)
            {
                Value = Constants.HalfMinuteAmount;
            }
            else if (data.Length == 2)
            {
                if (data[1] == Constants.HalfMinute)
                {
                    Value = TimeSpan.FromMinutes(double.Parse(data[0].ToString())).Add(Constants.HalfMinuteAmount);
                }
                else
                {
                    Value = TimeSpan.FromMinutes(double.Parse(data));
                }
            }
            else
            {
                Value = null;
            }
        }
    }

    public enum Activity
    {
        StopsOrShuntsForOtherTrainsToPass,
        AttachDetatchAssitingLocomotive,
        StopsForBankingLocomotive,
        StopsToChangeTrainmen,
        StopsToSetDownPassengers,
        StopsToDetatchVehicles,
        StopsForExamination,
        NRTDataToAdd,
        /// <summary>
        /// H or HH
        /// </summary>
        NotionalActivity,
        PassengerCountPoint,
        TicketCollectionAndExaminationPoint,
        TicketExaminationPoint,
        TicketExaminationPoint1stOnly,
        SelectiveTicketExaminationPoint,
        StopsToChangeLocomotives,
        StopNotAdvertised,
        StopsForOtherOperatingReasons,
        TrainLocomotiveOnRear,
        PopellingBetweenPointsShown,
        StopsWhenRequired,
        ReversingMovementOrDriverChangeEnds,
        StopsForLocomotiveToRunAround,
        StopsForRailwayPersonnelOnly,
        StopsToTakeUpAndSetDownPassengers,
        StopsToAttachAndDetachVehicles,
        TrainBegins,
        TrainFinishes,
        TOPSDirectRequestedByEWS,
        StopsForTabletStaffToken,
        StopsToTakeUpPassengers,
        StopsToAttachVehicles,
        StopsForWateringOfCoaches,
        PassesAnotherTrainAtCrossingPointOnSingleLine
    }

    public sealed class ActivityField : EnumField<Activity?>
    {
        public static readonly ActivityField Default = new ActivityField();

        public static Activity? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public ActivityField()
            : base(2, new Dictionary<string, Activity?>{
                { "A", Activity.StopsOrShuntsForOtherTrainsToPass },
                { "AE", Activity.AttachDetatchAssitingLocomotive },
                { "BL", Activity.StopsForBankingLocomotive },
                { "C", Activity.StopsToChangeTrainmen },
                { "D", Activity.StopsToSetDownPassengers },
                { "-D", Activity.StopsToDetatchVehicles },
                { "E", Activity.StopsForExamination },
                { "G", Activity.NRTDataToAdd },
                { "H", Activity.NotionalActivity },
                { "HH", Activity.NotionalActivity },
                { "K", Activity.PassengerCountPoint },
                { "KC", Activity.TicketCollectionAndExaminationPoint },
                { "KE", Activity.TicketExaminationPoint },
                { "KF", Activity.TicketExaminationPoint1stOnly },
                { "KS", Activity.SelectiveTicketExaminationPoint },
                { "L", Activity.StopsToChangeLocomotives },
                { "N", Activity.StopNotAdvertised },
                { "OP", Activity.StopsForOtherOperatingReasons },
                { "OR", Activity.TrainLocomotiveOnRear },
                { "PR", Activity.PopellingBetweenPointsShown },
                { "R", Activity.StopsWhenRequired },
                { "RM", Activity.ReversingMovementOrDriverChangeEnds },
                { "RR", Activity.StopsForLocomotiveToRunAround },
                { "S", Activity.StopsForRailwayPersonnelOnly },
                { "T", Activity.StopsToTakeUpAndSetDownPassengers },
                { "-T", Activity.StopsToAttachAndDetachVehicles },
                { "TB", Activity.TrainBegins },
                { "TF", Activity.TrainFinishes },
                { "TS", Activity.TOPSDirectRequestedByEWS },
                { "TW", Activity.StopsForTabletStaffToken },
                { "U", Activity.StopsToTakeUpPassengers },
                { "-U", Activity.StopsToAttachVehicles },
                { "W", Activity.StopsForWateringOfCoaches },
                { "X", Activity.PassesAnotherTrainAtCrossingPointOnSingleLine }
            }) { }
    }

    public enum ATSCode
    {
        SubjectToPerfMonitoring,
        NotSubjectToPerfMonitoring
    }

    public sealed class ATSCodeField : EnumField<ATSCode?>
    {
        public static readonly ATSCodeField Default = new ATSCodeField();

        public static ATSCode? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public ATSCodeField()
            : base(1, new Dictionary<string, ATSCode?>
            {
                { "Y", ATSCode.SubjectToPerfMonitoring },
                { "N", ATSCode.NotSubjectToPerfMonitoring }
            }) { }
    }

    public enum AssociationCategory
    {
        Join,
        Divide,
        Next
    }

    public sealed class AssociationCategoryField : EnumField<AssociationCategory?>
    {
        public static readonly AssociationCategoryField Default = new AssociationCategoryField();

        public static AssociationCategory? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public AssociationCategoryField()
            : base(2, new Dictionary<string, AssociationCategory?>
            {
                { "JJ", AssociationCategory.Join },
                { "VV", AssociationCategory.Divide },
                { "NP", AssociationCategory.Next }
            }) { }
    }

    public enum AssociationDateIndicator
    {
        Standard,
        OverNextMidnight,
        OverPreviousMidnight
    }

    public sealed class AssociationDateIndicatorField : EnumField<AssociationDateIndicator?>
    {
        public static readonly AssociationDateIndicatorField Default = new AssociationDateIndicatorField();

        public static AssociationDateIndicator? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public AssociationDateIndicatorField()
            : base(1, new Dictionary<string, AssociationDateIndicator?>
            {
                { "S", AssociationDateIndicator.Standard },
                { "N", AssociationDateIndicator.OverNextMidnight },
                { "P", AssociationDateIndicator.OverPreviousMidnight }
            }) { }
    }

    public enum AssociationType
    {
        Passenger,
        OperatingUse
    }

    public sealed class AssociationTypeField : EnumField<AssociationType?>
    {
        public static readonly AssociationTypeField Default = new AssociationTypeField();

        public static AssociationType? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public AssociationTypeField()
            : base(1, new Dictionary<string, AssociationType?>
            {
                { "P", AssociationType.Passenger },
                { "O", AssociationType.OperatingUse }
            }) { }
    }

    public enum CateType
    {
        NotInterchange = 0,
        SmallInterchange = 1,
        MediumInterchange = 2,
        LargeInterchange = 3,
        Subsiduary = 9
    }

    public sealed class CateTypeField : EnumField<CateType?>
    {
        public static readonly CateTypeField Default = new CateTypeField();

        public static CateType? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public CateTypeField()
            : base(1, new Dictionary<string, CateType?>{
                { "0", CateType.NotInterchange },
                { "1", CateType.SmallInterchange },
                { "2", CateType.MediumInterchange },
                { "3", CateType.LargeInterchange },
                { "9", CateType.Subsiduary }
            }, defaultValue: CateType.NotInterchange) { }
    }

    public static class Constants
    {
        public const string OngoingEndDate = "999999";
        public const char HalfMinute = 'H';
        public static readonly TimeSpan HalfMinuteAmount = TimeSpan.FromMinutes(0.5);
    }

    public enum StopType
    {
        Origin,
        Intermediate,
        Terminate
    }

    public sealed class StopTypeField : EnumField<StopType>
    {
        public static readonly StopTypeField Default = new StopTypeField();

        public static StopType? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public StopTypeField()
            : base(2, new Dictionary<string, StopType>{
                { "LO", StopType.Origin },
                { "LI", StopType.Intermediate },
                { "LT", StopType.Terminate }
            }, defaultValue: StopType.Intermediate) { }
    }
}
