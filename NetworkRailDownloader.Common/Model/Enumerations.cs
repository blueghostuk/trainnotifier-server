using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model
{
    public enum TransactionType
    {
        New,
        Delete,
        Revise
    }

    public sealed class TransactionTypeField : EnumField<TransactionType?>
    {
        public TransactionTypeField()
            : base(1, new Dictionary<string, TransactionType?>
            {
                { "N", TransactionType.New },
                { "D", TransactionType.Delete },
                { "R", TransactionType.Revise },
            }) { }
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
        public BankHolidayRunningField()
            : base(1, new Dictionary<string, BankHolidayRunning?>
            {
                { "X", BankHolidayRunning.DoesNotRunOnBankHolidayMondays },
                //{ "E", BankHolidayRunning.DoesNotRunOnEdinburghHolidays },
                { "G", BankHolidayRunning.DoesNotRunOnGlasgowHolidays },
            }, defaultValue: BankHolidayRunning.Runs) { }
    }

    public enum Status
    {
        Bus,
        Freight,
        PassengerAndParcels,
        Ship,
        Trip,
        STPPassengerAndParcels,
        STPFreight,
        STPTrip,
        STPShip,
        STPBus
    }

    public sealed class StatusField : EnumField<Status?>
    {
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

    public enum TrainCategory
    {
        LondonUndergroundMetro,
        UnadvertisedPassenger,
        OrdinaryPassenger,
        Staff,
        Mixed,
        ChannelTunnel,
        SleeperEurope,
        International,
        Motorail,
        UnadvertisedExpress,
        ExpressPassenger,
        SleeperDomestic,
        BusReplacementEngineering,
        BusWTT,
        ECS,
        ECSLondonUndergroundMetro,
        ECSStaff,
        Postal,
        PostOfficeParcels,
        Parcels,
        EmptyNPCCS,
        Departmental,
        CivilEngineer,
        MechanicalAndElectricalEngineer,
        Stores,
        Test,
        SignalAndTelecommsEngineer,
        LocoAndBrake,
        LightLoco,
        RfDAutomotiveComponents,
        RfDAutomotiveVehicles,
        RfDEdible,
        RfDIndustrialMinerals,
        RfDChemicals,
        RfDBuildingMaterials,
        RfDGeneralMerch,
        RfDEuropean,
        RfDFreightlinerContract,
        RfDFreightlinerOther,
        CoalDistributive,
        CoalElectrical,
        CoalAndNuclear,
        Metals,
        Aggreggates,
        DomesticAndIndustrialWaste,
        BuildingMaterials,
        Petroleum,
        RfDEuroChannelTunnelMixed,
        RfDEuroChannelTunnelIntermodal,
        RfDEuroChannelTunnelAutomotive,
        RfDEuroChannelTunnelContractServices,
        RfDEuroChannelTunnelHaulmark,
        RfDEuroChannelTunnelJointVenture
    }

    public sealed class TrainCategoryField : EnumField<TrainCategory?>
    {
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

    public enum PowerType
    {
        Diesel,
        DieselElectricMultipleUnit,
        DieselMechanicalMultipleUnit,
        Electric,
        ElectroDiesel,
        EML,
        ElectricMultipleUnit,
        ElectricParcelsUnit,
        HighSpeedTrain,
        DieselShuntingLocomotive
    }

    public sealed class PowerTypeField : EnumField<PowerType?>
    {
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
        public TrainClassField()
            : base(1, new Dictionary<string, TrainClass?>{
                { "B", TrainClass.FirstAndStandard },
                { "S", TrainClass.StandardOnly },
                { "F", TrainClass.FirstOnly }
            }, defaultValue: TrainClass.FirstAndStandard) { }
    }

    public sealed class SleepersField : EnumField<TrainClass?>
    {
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
        public ServiceBrandingField()
            : base(1, new Dictionary<string, ServiceBranding?>{
                { "E", ServiceBranding.Eurostar },
                { "U", ServiceBranding.Alphaline }
            }) { }
    }

    public enum STPIndicator
    {
        CancellationOfPermanentSchedule,
        NewSchedule,
        Overlay,
        Permanent
    }

    public sealed class STPIndicatorField : EnumField<STPIndicator?>
    {
        public STPIndicatorField()
            : base(1, new Dictionary<string, STPIndicator?>{
                { "C", STPIndicator.CancellationOfPermanentSchedule },
                { "N", STPIndicator.NewSchedule },
                { "O", STPIndicator.Overlay },
                { "P", STPIndicator.Permanent }
            }) { }
    }

    /// <remarks>
    /// Also used for Performance Allowance
    /// </remarks>
    public sealed class EngineeringAllowanceField : RecordField<TimeSpan?>
    {
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
}
