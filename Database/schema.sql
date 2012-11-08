-- --------------------------------------------------------
-- HeidiSQL version:             7.0.0.4053
-- Date/time:                    2012-11-08 23:36:50
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET FOREIGN_KEY_CHECKS=0 */;

-- Dumping database structure for natrail
CREATE DATABASE IF NOT EXISTS `natrail` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `natrail`;


-- Dumping structure for table natrail.Activity
CREATE TABLE IF NOT EXISTS `Activity` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(50) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.Catering
CREATE TABLE IF NOT EXISTS `Catering` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(50) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.CateType
CREATE TABLE IF NOT EXISTS `CateType` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(20) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.ChangeEnRouteLocation
CREATE TABLE IF NOT EXISTS `ChangeEnRouteLocation` (
  `LocationId` int(10) unsigned NOT NULL,
  `TrainCategoryId` tinyint(3) unsigned default NULL,
  `HeadCode` varchar(4) default NULL,
  `TrainServiceCode` varchar(8) default NULL,
  `PowerTypeId` tinyint(3) unsigned default NULL,
  `TimingLoad` varchar(4) default NULL,
  `Speed` tinyint(3) unsigned default NULL,
  `TrainClass` tinyint(3) unsigned default NULL,
  `Sleepers` tinyint(3) unsigned default NULL,
  `Reservations` tinyint(3) unsigned default NULL,
  `UICCode` varchar(5) default NULL,
  PRIMARY KEY  (`LocationId`),
  KEY `ChangeEnRouteLocation_TrainCategory_Id` (`TrainCategoryId`),
  KEY `ChangeEnRouteLocation_PowerType_Id` (`PowerTypeId`),
  KEY `ChangeEnRouteLocation_TrainClass_Id` (`TrainClass`),
  KEY `ChangeEnRouteLocation_Sleepers_Id` (`Sleepers`),
  KEY `ChangeEnRouteLocation_Reservations_Id` (`Reservations`),
  CONSTRAINT `ChangeEnRouteLocation_Location_Id` FOREIGN KEY (`LocationId`) REFERENCES `Location` (`Id`),
  CONSTRAINT `ChangeEnRouteLocation_PowerType_Id` FOREIGN KEY (`PowerTypeId`) REFERENCES `PowerType` (`Id`),
  CONSTRAINT `ChangeEnRouteLocation_Reservations_Id` FOREIGN KEY (`Reservations`) REFERENCES `Reservation` (`Id`),
  CONSTRAINT `ChangeEnRouteLocation_Sleepers_Id` FOREIGN KEY (`Sleepers`) REFERENCES `TrainClass` (`Id`),
  CONSTRAINT `ChangeEnRouteLocation_TrainCategory_Id` FOREIGN KEY (`TrainCategoryId`) REFERENCES `TrainCategory` (`Id`),
  CONSTRAINT `ChangeEnRouteLocation_TrainClass_Id` FOREIGN KEY (`TrainClass`) REFERENCES `TrainClass` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.ChangeEnRouteLocationCatering
CREATE TABLE IF NOT EXISTS `ChangeEnRouteLocationCatering` (
  `LocationId` int(10) unsigned NOT NULL,
  `CateringId` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`LocationId`,`CateringId`),
  KEY `ChangeEnRouteLocationCatering_Catering_Id` (`CateringId`),
  CONSTRAINT `ChangeEnRouteLocationCatering_Catering_Id` FOREIGN KEY (`CateringId`) REFERENCES `Catering` (`Id`),
  CONSTRAINT `ChangeEnRouteLocationCatering_Location_Id` FOREIGN KEY (`LocationId`) REFERENCES `ChangeEnRouteLocation` (`LocationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.ChangeEnRouteLocationOperatingCharacteristics
CREATE TABLE IF NOT EXISTS `ChangeEnRouteLocationOperatingCharacteristics` (
  `LocationId` int(10) unsigned NOT NULL,
  `OperatingCharacteristicId` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`LocationId`,`OperatingCharacteristicId`),
  KEY `ChangeEnRouteLocationOpCharacteristics_OpCharacteristic_Id` (`OperatingCharacteristicId`),
  CONSTRAINT `ChangeEnRouteLocationOpCharacteristics_Location_Id` FOREIGN KEY (`LocationId`) REFERENCES `ChangeEnRouteLocation` (`LocationId`),
  CONSTRAINT `ChangeEnRouteLocationOpCharacteristics_OpCharacteristic_Id` FOREIGN KEY (`OperatingCharacteristicId`) REFERENCES `OperatingCharacteristic` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.ChangeEnRouteLocationServiceBranding
CREATE TABLE IF NOT EXISTS `ChangeEnRouteLocationServiceBranding` (
  `LocationId` int(10) unsigned NOT NULL,
  `ServiceBrandingId` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`LocationId`,`ServiceBrandingId`),
  KEY `ChangeEnRouteLocationServiceBranding_ServiceBranding_Id` (`ServiceBrandingId`),
  CONSTRAINT `ChangeEnRouteLocationServiceBranding_Location_Id` FOREIGN KEY (`LocationId`) REFERENCES `ChangeEnRouteLocation` (`LocationId`),
  CONSTRAINT `ChangeEnRouteLocationServiceBranding_ServiceBranding_Id` FOREIGN KEY (`ServiceBrandingId`) REFERENCES `ServiceBranding` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.Day
CREATE TABLE IF NOT EXISTS `Day` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(10) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.IntermediateLocation
CREATE TABLE IF NOT EXISTS `IntermediateLocation` (
  `LocationId` int(10) unsigned NOT NULL,
  `ScheduledArrival` time default NULL,
  `ScheduledDeparture` time default NULL,
  `ScheduledPass` time default NULL,
  `PublicArrival` time default NULL,
  `PublicDeparture` time default NULL,
  `Platform` varchar(3) default NULL,
  `Line` varchar(3) default NULL,
  `Path` varchar(3) default NULL,
  `EngineeringAllowance` time default NULL,
  `PathingAllowance` time default NULL,
  `PerformanceAllowance` time default NULL,
  PRIMARY KEY  (`LocationId`),
  CONSTRAINT `IntermediateLocation_Location_Id` FOREIGN KEY (`LocationId`) REFERENCES `Location` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.LiveTrain
CREATE TABLE IF NOT EXISTS `LiveTrain` (
  `TrainId` char(10) NOT NULL,
  `creation_timestamp` datetime default NULL,
  `origin_dep_timestamp` datetime default NULL,
  `train_service_code` varchar(50) default NULL,
  `toc_id` varchar(50) default NULL,
  `train_uid` varchar(50) default NULL,
  `sched_origin_stanox` varchar(50) default NULL,
  `sched_wtt_id` varchar(50) default NULL
) ENGINE=ARCHIVE DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.LiveTrainCancellation
CREATE TABLE IF NOT EXISTS `LiveTrainCancellation` (
  `TrainId` varchar(50) NOT NULL,
  `canx_timestamp` datetime NOT NULL,
  `train_service_code` varchar(50) NOT NULL,
  `loc_stanox` varchar(50) default NULL,
  `canx_reason_code` varchar(50) default NULL,
  `canx_type` varchar(50) default NULL
) ENGINE=ARCHIVE DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.LiveTrainStop
CREATE TABLE IF NOT EXISTS `LiveTrainStop` (
  `TrainId` varchar(50) NOT NULL,
  `event_type` varchar(50) default NULL,
  `planned_timestamp` datetime default NULL,
  `actual_timestamp` datetime default NULL,
  `reporting_stanox` varchar(50) default NULL,
  `platform` varchar(50) default NULL,
  `line` varchar(50) default NULL,
  `train_terminated` tinyint(3) unsigned NOT NULL default '0'
) ENGINE=ARCHIVE DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.Location
CREATE TABLE IF NOT EXISTS `Location` (
  `Id` int(10) unsigned NOT NULL auto_increment,
  `TypeId` tinyint(3) unsigned NOT NULL,
  `Location` smallint(5) unsigned NOT NULL,
  PRIMARY KEY  (`Id`),
  KEY `Location_LocationType_TypeId` (`TypeId`),
  CONSTRAINT `Location_LocationType_TypeId` FOREIGN KEY (`TypeId`) REFERENCES `LocationType` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.LocationActivity
CREATE TABLE IF NOT EXISTS `LocationActivity` (
  `LocationId` int(10) unsigned NOT NULL,
  `ActivityId` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`LocationId`,`ActivityId`),
  KEY `LocationActivity_Activity_Id` (`ActivityId`),
  CONSTRAINT `LocationActivity_Activity_Id` FOREIGN KEY (`ActivityId`) REFERENCES `Activity` (`Id`),
  CONSTRAINT `LocationActivity_Location_Id` FOREIGN KEY (`LocationId`) REFERENCES `Location` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.LocationType
CREATE TABLE IF NOT EXISTS `LocationType` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(50) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.OperatingCharacteristic
CREATE TABLE IF NOT EXISTS `OperatingCharacteristic` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(50) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.OriginLocation
CREATE TABLE IF NOT EXISTS `OriginLocation` (
  `LocationId` int(10) unsigned NOT NULL,
  `ScheduledDeparture` time NOT NULL,
  `PublicDeparture` time NOT NULL,
  `Platform` varchar(3) NOT NULL,
  `Line` varchar(3) NOT NULL,
  `EngineeringAllowance` time default NULL,
  `PathingAllowance` time default NULL,
  `PerformanceAllowance` time default NULL,
  PRIMARY KEY  (`LocationId`),
  CONSTRAINT `OriginLocation_Location_Id` FOREIGN KEY (`LocationId`) REFERENCES `Location` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.PowerType
CREATE TABLE IF NOT EXISTS `PowerType` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(50) default NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.Reservation
CREATE TABLE IF NOT EXISTS `Reservation` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(50) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.RoutingGroup
CREATE TABLE IF NOT EXISTS `RoutingGroup` (
  `Id` smallint(5) unsigned NOT NULL auto_increment,
  `Name` varchar(30) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.RoutingGroupStation
CREATE TABLE IF NOT EXISTS `RoutingGroupStation` (
  `RoutingGroupId` smallint(5) unsigned NOT NULL,
  `StationId` smallint(5) unsigned NOT NULL,
  PRIMARY KEY  (`RoutingGroupId`,`StationId`),
  KEY `RoutingGroupStation_Station_Id` (`StationId`),
  CONSTRAINT `RoutingGroupStation_RoutingGroup_Id` FOREIGN KEY (`RoutingGroupId`) REFERENCES `RoutingGroup` (`Id`),
  CONSTRAINT `RoutingGroupStation_Station_Id` FOREIGN KEY (`StationId`) REFERENCES `Station` (`StationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.Schedule
CREATE TABLE IF NOT EXISTS `Schedule` (
  `Id` int(10) unsigned NOT NULL auto_increment,
  `TrainUID` varchar(6) NOT NULL,
  `StartDate` datetime NOT NULL,
  `EndDate` datetime default NULL,
  `Status` tinyint(3) unsigned default NULL,
  `Category` tinyint(3) unsigned default NULL,
  `Identity` varchar(4) default NULL,
  `HeadCode` varchar(4) default NULL,
  `TrainServiceCode` varchar(8) default NULL,
  `PortionId` varchar(1) default NULL,
  `PowerType` tinyint(3) unsigned default NULL,
  `TimingLoad` varchar(50) default NULL,
  `Speed` tinyint(3) unsigned default NULL,
  `TrainClass` tinyint(3) unsigned default NULL,
  `SleepersClass` tinyint(3) unsigned default NULL,
  `Reservations` tinyint(3) unsigned default NULL,
  PRIMARY KEY  (`Id`),
  KEY `Schedule_TrainUID_UID` (`TrainUID`),
  KEY `Schedule_Status_Id` (`Status`),
  KEY `Schedule_Category_Id` (`Category`),
  KEY `Schedule_PowerType_Id` (`PowerType`),
  KEY `Schedule_TrainClass_Id` (`TrainClass`),
  KEY `Schedule_SleepersClass_Id` (`SleepersClass`),
  KEY `Schedule_Reservations_Id` (`Reservations`),
  CONSTRAINT `Schedule_Category_Id` FOREIGN KEY (`Category`) REFERENCES `TrainCategory` (`Id`),
  CONSTRAINT `Schedule_PowerType_Id` FOREIGN KEY (`PowerType`) REFERENCES `PowerType` (`Id`),
  CONSTRAINT `Schedule_Reservations_Id` FOREIGN KEY (`Reservations`) REFERENCES `Reservation` (`Id`),
  CONSTRAINT `Schedule_SleepersClass_Id` FOREIGN KEY (`SleepersClass`) REFERENCES `TrainClass` (`Id`),
  CONSTRAINT `Schedule_Status_Id` FOREIGN KEY (`Status`) REFERENCES `Status` (`Id`),
  CONSTRAINT `Schedule_TrainClass_Id` FOREIGN KEY (`TrainClass`) REFERENCES `TrainClass` (`Id`),
  CONSTRAINT `Schedule_TrainUID_UID` FOREIGN KEY (`TrainUID`) REFERENCES `Train` (`UID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.ScheduleBranding
CREATE TABLE IF NOT EXISTS `ScheduleBranding` (
  `ScheduleId` int(10) unsigned NOT NULL,
  `ServiceBrandingId` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`ScheduleId`,`ServiceBrandingId`),
  KEY `ServiceBranding_ServiceBranding_Id` (`ServiceBrandingId`),
  CONSTRAINT `ServiceBranding_Schedule_Id` FOREIGN KEY (`ScheduleId`) REFERENCES `Schedule` (`Id`),
  CONSTRAINT `ServiceBranding_ServiceBranding_Id` FOREIGN KEY (`ServiceBrandingId`) REFERENCES `ServiceBranding` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.ScheduleCatering
CREATE TABLE IF NOT EXISTS `ScheduleCatering` (
  `ScheduleId` int(10) unsigned NOT NULL,
  `CateringId` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`ScheduleId`,`CateringId`),
  KEY `ScheduleCatering_Catering_Id` (`CateringId`),
  CONSTRAINT `ScheduleCatering_Catering_Id` FOREIGN KEY (`CateringId`) REFERENCES `Catering` (`Id`),
  CONSTRAINT `ScheduleCatering_Schedule_Id` FOREIGN KEY (`ScheduleId`) REFERENCES `Schedule` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.ScheduleDay
CREATE TABLE IF NOT EXISTS `ScheduleDay` (
  `ScheduleId` int(10) unsigned NOT NULL,
  `DayId` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`ScheduleId`,`DayId`),
  KEY `ScheduleDay_Day_Id` (`DayId`),
  CONSTRAINT `ScheduleDay_Day_Id` FOREIGN KEY (`DayId`) REFERENCES `Day` (`Id`),
  CONSTRAINT `ScheduleDay_Schedule_Id` FOREIGN KEY (`ScheduleId`) REFERENCES `Schedule` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.ScheduleOperatingCharacteristic
CREATE TABLE IF NOT EXISTS `ScheduleOperatingCharacteristic` (
  `ScheduleId` int(10) unsigned NOT NULL,
  `OperatingCharacteristicId` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`ScheduleId`,`OperatingCharacteristicId`),
  KEY `ScheduleOperatingCharacteristic_OperatingCharacteristic` (`OperatingCharacteristicId`),
  CONSTRAINT `ScheduleOperatingCharacteristic_OperatingCharacteristic` FOREIGN KEY (`OperatingCharacteristicId`) REFERENCES `OperatingCharacteristic` (`Id`),
  CONSTRAINT `ScheduleOperatingCharacteristic_Schedule` FOREIGN KEY (`ScheduleId`) REFERENCES `Schedule` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.ServiceBranding
CREATE TABLE IF NOT EXISTS `ServiceBranding` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(50) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.Station
CREATE TABLE IF NOT EXISTS `Station` (
  `StationId` smallint(5) unsigned NOT NULL auto_increment,
  `TiplocId` smallint(5) unsigned NOT NULL,
  `StationName` varchar(30) NOT NULL,
  `CATEType` tinyint(3) unsigned NOT NULL,
  `SubsiduaryAlphaCode` varchar(3) default NULL,
  `AlphaCode` varchar(3) NOT NULL,
  `Location` geometry NOT NULL,
  `LocationEstimated` tinyint(1) unsigned NOT NULL,
  `ChangeTime` time default NULL,
  `ManualEntry` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`StationId`),
  UNIQUE KEY `TiplocId_StationName` (`TiplocId`,`StationName`),
  KEY `StationName` (`StationName`),
  KEY `AlphaCode` (`AlphaCode`),
  KEY `SubsiduaryAlphaCode` (`SubsiduaryAlphaCode`),
  CONSTRAINT `Station_Tiploc_Id` FOREIGN KEY (`TiplocId`) REFERENCES `Tiploc` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.StationAlias
CREATE TABLE IF NOT EXISTS `StationAlias` (
  `StationAliasId` smallint(5) unsigned NOT NULL auto_increment,
  `StationId` smallint(5) unsigned NOT NULL,
  `Alias` varchar(30) NOT NULL,
  PRIMARY KEY  (`StationAliasId`),
  UNIQUE KEY `StationId_Alias` (`StationId`,`Alias`),
  CONSTRAINT `StationAlias_Station_Id` FOREIGN KEY (`StationId`) REFERENCES `Station` (`StationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.Status
CREATE TABLE IF NOT EXISTS `Status` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(20) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.TerminatingLocation
CREATE TABLE IF NOT EXISTS `TerminatingLocation` (
  `LocationId` int(10) unsigned NOT NULL,
  `ScheduledArrival` time NOT NULL,
  `PublicArrival` time NOT NULL,
  `Platform` varchar(3) NOT NULL,
  `Path` varchar(3) NOT NULL,
  PRIMARY KEY  (`LocationId`),
  CONSTRAINT `TerminatingLocation_Location_Id` FOREIGN KEY (`LocationId`) REFERENCES `Location` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.Tiploc
CREATE TABLE IF NOT EXISTS `Tiploc` (
  `Id` smallint(5) unsigned NOT NULL auto_increment,
  `Tiploc` varchar(7) NOT NULL,
  `Nalco` varchar(6) default NULL,
  `Description` varchar(26) default NULL,
  `Stanox` varchar(5) default NULL,
  `CRS` varchar(3) default NULL,
  `Deleted` tinyint(1) unsigned NOT NULL default '0',
  `DateDeleted` datetime default NULL,
  PRIMARY KEY  (`Id`),
  UNIQUE KEY `Tiploc` (`Tiploc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.Train
CREATE TABLE IF NOT EXISTS `Train` (
  `UID` varchar(6) NOT NULL,
  PRIMARY KEY  (`UID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.TrainCategory
CREATE TABLE IF NOT EXISTS `TrainCategory` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(50) NOT NULL,
  `Default` tinyint(1) unsigned NOT NULL default '0',
  `DoNotOutput` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.


-- Dumping structure for table natrail.TrainClass
CREATE TABLE IF NOT EXISTS `TrainClass` (
  `Id` tinyint(3) unsigned NOT NULL,
  `Description` varchar(50) NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.
/*!40014 SET FOREIGN_KEY_CHECKS=1 */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
