SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AtocCode]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AtocCode](
	[AtocCode] [nvarchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_AtocCode] PRIMARY KEY CLUSTERED 
(
	[AtocCode] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DelayAttributionCodes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DelayAttributionCodes](
	[ReasonCode] [nchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [nvarchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DelayAttributionCodes] PRIMARY KEY CLUSTERED 
(
	[ReasonCode] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tiploc]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Tiploc](
	[TiplocId] [smallint] IDENTITY(1,1) NOT NULL,
	[Tiploc] [nvarchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Nalco] [nvarchar](6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [nvarchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Stanox] [nvarchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CRS] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Tiploc] PRIMARY KEY CLUSTERED 
(
	[TiplocId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Tiploc]') AND name = N'''IX_Tiploc_CRS''')
CREATE NONCLUSTERED INDEX ['IX_Tiploc_CRS'] ON [dbo].[Tiploc] 
(
	[CRS] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Tiploc]') AND name = N'''IX_Tiploc_Stanox''')
CREATE NONCLUSTERED INDEX ['IX_Tiploc_Stanox'] ON [dbo].[Tiploc] 
(
	[Stanox] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Tiploc_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tiploc] ADD  CONSTRAINT [DF_Tiploc_Deleted]  DEFAULT ((0)) FOR [Deleted]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LiveTrain]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LiveTrain](
	[Id] [uniqueidentifier] NOT NULL,
	[TrainId] [nvarchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Headcode] [nvarchar](4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[OriginDepartTimestamp] [datetime] NOT NULL,
	[TrainServiceCode] [nvarchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OriginTiplocId] [smallint] NOT NULL,
	[TrainStateId] [tinyint] NOT NULL,
	[ScheduleTrain] [uniqueidentifier] NULL,
 CONSTRAINT [PK_LiveTrain] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[LiveTrain]') AND name = N'IX_LiveTrain_Lookup')
CREATE NONCLUSTERED INDEX [IX_LiveTrain_Lookup] ON [dbo].[LiveTrain] 
(
	[ScheduleTrain] ASC,
	[OriginDepartTimestamp] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_LiveTrain_Id]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LiveTrain] ADD  CONSTRAINT [DF_LiveTrain_Id]  DEFAULT (newid()) FOR [Id]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_LiveTrain_TrainStateId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LiveTrain] ADD  CONSTRAINT [DF_LiveTrain_TrainStateId]  DEFAULT ((0)) FOR [TrainStateId]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LiveTrainCancellation]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LiveTrainCancellation](
	[TrainCancellationId] [uniqueidentifier] NOT NULL,
	[TrainId] [uniqueidentifier] NOT NULL,
	[CancelledTimestamp] [datetime] NOT NULL,
	[Stanox] [nvarchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ReasonCode] [nchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Type] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_LiveTrainCancellation] PRIMARY KEY CLUSTERED 
(
	[TrainCancellationId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[LiveTrainCancellation]') AND name = N'IX_LiveTrainCancellation_Lookup')
CREATE NONCLUSTERED INDEX [IX_LiveTrainCancellation_Lookup] ON [dbo].[LiveTrainCancellation] 
(
	[TrainId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_LiveTrainCancellation_TrainCancellationId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LiveTrainCancellation] ADD  CONSTRAINT [DF_LiveTrainCancellation_TrainCancellationId]  DEFAULT (newid()) FOR [TrainCancellationId]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LiveTrainChangeOfOrigin]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LiveTrainChangeOfOrigin](
	[TrainChangeOfOriginId] [uniqueidentifier] NOT NULL,
	[TrainId] [uniqueidentifier] NOT NULL,
	[ReasonCode] [nchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[NewTiplocId] [smallint] NOT NULL,
	[NewDepartureTime] [datetime] NOT NULL,
	[ChangedTime] [datetime] NULL,
 CONSTRAINT [PK_LiveTrainChangeOfOrigin] PRIMARY KEY CLUSTERED 
(
	[TrainChangeOfOriginId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_LiveTrainChangeOfOrigin_TrainChangeOfOriginId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LiveTrainChangeOfOrigin] ADD  CONSTRAINT [DF_LiveTrainChangeOfOrigin_TrainChangeOfOriginId]  DEFAULT (newid()) FOR [TrainChangeOfOriginId]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LiveTrainReinstatement]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LiveTrainReinstatement](
	[TrainReinstatementId] [uniqueidentifier] NOT NULL,
	[TrainId] [uniqueidentifier] NOT NULL,
	[PlannedDepartureTime] [datetime] NOT NULL,
	[ReinstatedTiplocId] [smallint] NOT NULL,
	[ReinstatementTime] [datetime] NULL,
 CONSTRAINT [PK_LiveTrainReinstatement] PRIMARY KEY CLUSTERED 
(
	[TrainReinstatementId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_LiveTrainReinstatement_TrainReinstatementId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LiveTrainReinstatement] ADD  CONSTRAINT [DF_LiveTrainReinstatement_TrainReinstatementId]  DEFAULT (newid()) FOR [TrainReinstatementId]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LiveTrainStop]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LiveTrainStop](
	[TrainId] [uniqueidentifier] NOT NULL,
	[EventTypeId] [tinyint] NOT NULL,
	[PlannedTimestamp] [datetime] NULL,
	[ActualTimestamp] [datetime] NOT NULL,
	[ReportingTiplocId] [smallint] NOT NULL,
	[Platform] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Line] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ScheduleStopNumber] [tinyint] NULL
)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[LiveTrainStop]') AND name = N'IX_LiveTrainStop')
CREATE CLUSTERED INDEX [IX_LiveTrainStop] ON [dbo].[LiveTrainStop] 
(
	[TrainId] ASC,
	[EventTypeId] ASC,
	[ActualTimestamp] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PPMRecord]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PPMRecord](
	[PPMSectorId] [uniqueidentifier] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Total] [smallint] NOT NULL,
	[OnTime] [smallint] NOT NULL,
	[Late] [smallint] NOT NULL,
	[CancelVeryLate] [smallint] NOT NULL,
	[Trend] [tinyint] NOT NULL,
 CONSTRAINT [PK_PPMRecord] PRIMARY KEY CLUSTERED 
(
	[PPMSectorId] ASC,
	[Timestamp] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PPMSectors]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PPMSectors](
	[PPMSectorId] [uniqueidentifier] NOT NULL,
	[OperatorCode] [tinyint] NULL,
	[SectorCode] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_PPMSectors] PRIMARY KEY CLUSTERED 
(
	[PPMSectorId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_PPMSectors_PPMSectorId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PPMSectors] ADD  CONSTRAINT [DF_PPMSectors_PPMSectorId]  DEFAULT (newid()) FOR [PPMSectorId]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleSource]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ScheduleSource](
	[ScheduleSourceId] [tinyint] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ScheduleSource] PRIMARY KEY CLUSTERED 
(
	[ScheduleSourceId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleStatus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ScheduleStatus](
	[StatusId] [tinyint] NOT NULL,
	[Code] [nvarchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ScheduleStatus] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrainCategory]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ScheduleTrainCategory](
	[TrainCategoryId] [tinyint] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ScheduleTrainCategory] PRIMARY KEY CLUSTERED 
(
	[TrainCategoryId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrainPowerType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ScheduleTrainPowerType](
	[PowerTypeId] [tinyint] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ScheduleTrainPowerType] PRIMARY KEY CLUSTERED 
(
	[PowerTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[STPIndicator]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[STPIndicator](
	[STPIndicatorId] [tinyint] NOT NULL,
	[Code] [nvarchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_STPIndicator] PRIMARY KEY CLUSTERED 
(
	[STPIndicatorId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ScheduleTrain](
	[ScheduleId] [uniqueidentifier] NOT NULL,
	[TrainUid] [nvarchar](6) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	[AtocCode] [nvarchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[RunsMonday] [bit] NOT NULL,
	[RunsTuesday] [bit] NOT NULL,
	[RunsWednesday] [bit] NOT NULL,
	[RunsThursday] [bit] NOT NULL,
	[RunsFriday] [bit] NOT NULL,
	[RunsSaturday] [bit] NOT NULL,
	[RunsSunday] [bit] NOT NULL,
	[RunsBankHoliday] [bit] NOT NULL,
	[STPIndicatorId] [tinyint] NOT NULL,
	[ScheduleStatusId] [tinyint] NULL,
	[OriginStopTiplocId] [smallint] NULL,
	[DestinationStopTiplocId] [smallint] NULL,
	[Deleted] [bit] NOT NULL,
	[Source] [tinyint] NOT NULL,
	[Headcode] [nvarchar](4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PowerTypeId] [tinyint] NULL,
	[CategoryTypeId] [tinyint] NULL,
	[Speed] [tinyint] NULL,
 CONSTRAINT [PK_ScheduleTrain] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_Schedule_Term_Tiploc')
CREATE NONCLUSTERED INDEX [IX_Schedule_Term_Tiploc] ON [dbo].[ScheduleTrain] 
(
	[RunsMonday] ASC,
	[DestinationStopTiplocId] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ( [ScheduleId],
[TrainUid],
[STPIndicatorId]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Friday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Friday] ON [dbo].[ScheduleTrain] 
(
	[RunsFriday] ASC,
	[OriginStopTiplocId] ASC,
	[DestinationStopTiplocId] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ( [ScheduleId],
[TrainUid],
[STPIndicatorId]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Monday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Monday] ON [dbo].[ScheduleTrain] 
(
	[RunsMonday] ASC,
	[OriginStopTiplocId] ASC,
	[DestinationStopTiplocId] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ( [ScheduleId],
[TrainUid],
[STPIndicatorId]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Saturday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Saturday] ON [dbo].[ScheduleTrain] 
(
	[RunsSaturday] ASC,
	[OriginStopTiplocId] ASC,
	[DestinationStopTiplocId] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ( [ScheduleId],
[TrainUid],
[STPIndicatorId]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Sunday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Sunday] ON [dbo].[ScheduleTrain] 
(
	[RunsSunday] ASC,
	[OriginStopTiplocId] ASC,
	[DestinationStopTiplocId] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ( [ScheduleId],
[TrainUid],
[STPIndicatorId]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Thursday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Thursday] ON [dbo].[ScheduleTrain] 
(
	[RunsThursday] ASC,
	[OriginStopTiplocId] ASC,
	[DestinationStopTiplocId] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ( [ScheduleId],
[TrainUid],
[STPIndicatorId]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Tuesday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Tuesday] ON [dbo].[ScheduleTrain] 
(
	[RunsTuesday] ASC,
	[OriginStopTiplocId] ASC,
	[DestinationStopTiplocId] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ( [ScheduleId],
[TrainUid],
[STPIndicatorId]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Wednesday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Wednesday] ON [dbo].[ScheduleTrain] 
(
	[RunsWednesday] ASC,
	[OriginStopTiplocId] ASC,
	[DestinationStopTiplocId] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ( [ScheduleId],
[TrainUid],
[STPIndicatorId]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleTrain_Lookup_Friday')
CREATE NONCLUSTERED INDEX [IX_ScheduleTrain_Lookup_Friday] ON [dbo].[ScheduleTrain] 
(
	[TrainUid] ASC,
	[RunsFriday] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleTrain_Lookup_Monday')
CREATE NONCLUSTERED INDEX [IX_ScheduleTrain_Lookup_Monday] ON [dbo].[ScheduleTrain] 
(
	[TrainUid] ASC,
	[RunsMonday] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleTrain_Lookup_Saturday')
CREATE NONCLUSTERED INDEX [IX_ScheduleTrain_Lookup_Saturday] ON [dbo].[ScheduleTrain] 
(
	[TrainUid] ASC,
	[RunsSaturday] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleTrain_Lookup_Sunday')
CREATE NONCLUSTERED INDEX [IX_ScheduleTrain_Lookup_Sunday] ON [dbo].[ScheduleTrain] 
(
	[TrainUid] ASC,
	[RunsSunday] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleTrain_Lookup_Thursday')
CREATE NONCLUSTERED INDEX [IX_ScheduleTrain_Lookup_Thursday] ON [dbo].[ScheduleTrain] 
(
	[TrainUid] ASC,
	[RunsThursday] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleTrain_Lookup_Tuesday')
CREATE NONCLUSTERED INDEX [IX_ScheduleTrain_Lookup_Tuesday] ON [dbo].[ScheduleTrain] 
(
	[TrainUid] ASC,
	[RunsTuesday] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleTrain_Lookup_Wednesday')
CREATE NONCLUSTERED INDEX [IX_ScheduleTrain_Lookup_Wednesday] ON [dbo].[ScheduleTrain] 
(
	[TrainUid] ASC,
	[RunsWednesday] ASC,
	[Deleted] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Headcode_Friday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Headcode_Friday] ON [dbo].[ScheduleTrain]
(
	[RunsFriday] ASC,
	[Headcode] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ([ScheduleId],
	[TrainUid],
	[STPIndicatorId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Headcode_Saturday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Headcode_Saturday] ON [dbo].[ScheduleTrain]
(
	[RunsSaturday] ASC,
	[Headcode] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ([ScheduleId],
	[TrainUid],
	[STPIndicatorId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Headcode_Sunday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Headcode_Sunday] ON [dbo].[ScheduleTrain]
(
	[RunsSunday] ASC,
	[Headcode] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ([ScheduleId],
	[TrainUid],
	[STPIndicatorId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Headcode_Monday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Headcode_Monday] ON [dbo].[ScheduleTrain]
(
	[RunsMonday] ASC,
	[Headcode] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ([ScheduleId],
	[TrainUid],
	[STPIndicatorId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Headcode_Tuesday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Headcode_Tuesday] ON [dbo].[ScheduleTrain]
(
	[RunsTuesday] ASC,
	[Headcode] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ([ScheduleId],
	[TrainUid],
	[STPIndicatorId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Headcode_Wednesday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Headcode_Wednesday] ON [dbo].[ScheduleTrain]
(
	[RunsWednesday] ASC,
	[Headcode] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ([ScheduleId],
	[TrainUid],
	[STPIndicatorId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]') AND name = N'IX_ScheduleLookup_Headcode_Thursday')
CREATE NONCLUSTERED INDEX [IX_ScheduleLookup_Headcode_Thursday] ON [dbo].[ScheduleTrain]
(
	[RunsThursday] ASC,
	[Headcode] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ([ScheduleId],
	[TrainUid],
	[STPIndicatorId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_ScheduleId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_ScheduleId]  DEFAULT (newid()) FOR [ScheduleId]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_RunsMonday]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsMonday]  DEFAULT ((0)) FOR [RunsMonday]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_RunsTuesday]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsTuesday]  DEFAULT ((0)) FOR [RunsTuesday]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_RunsWednesday]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsWednesday]  DEFAULT ((0)) FOR [RunsWednesday]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_RunsThursday]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsThursday]  DEFAULT ((0)) FOR [RunsThursday]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_RunsFriday]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsFriday]  DEFAULT ((0)) FOR [RunsFriday]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_RunsSaturday]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsSaturday]  DEFAULT ((0)) FOR [RunsSaturday]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_RunsSunday]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsSunday]  DEFAULT ((0)) FOR [RunsSunday]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_RunsBankHoliday]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsBankHoliday]  DEFAULT ((0)) FOR [RunsBankHoliday]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_Deleted]  DEFAULT ((0)) FOR [Deleted]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrain_Source]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_Source]  DEFAULT ((0)) FOR [Source]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrainStop]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ScheduleTrainStop](
	[ScheduleId] [uniqueidentifier] NOT NULL,
	[StopNumber] [tinyint] NOT NULL,
	[TiplocId] [smallint] NOT NULL,
	[Arrival] [time](0) NULL,
	[Departure] [time](0) NULL,
	[Pass] [time](0) NULL,
	[PublicArrival] [time](0) NULL,
	[PublicDeparture] [time](0) NULL,
	[Line] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Path] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Platform] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EngineeringAllowance] [tinyint] NULL,
	[PathingAllowance] [tinyint] NULL,
	[PerformanceAllowance] [tinyint] NULL,
	[Origin] [bit] NOT NULL,
	[Intermediate] [bit] NOT NULL,
	[Terminate] [bit] NOT NULL,
 CONSTRAINT [PK_ScheduleTrainStop] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC,
	[StopNumber] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleTrainStop]') AND name = N'IX_Schedule_At_Tiploc')
CREATE NONCLUSTERED INDEX [IX_Schedule_At_Tiploc] ON [dbo].[ScheduleTrainStop] 
(
	[TiplocId] ASC
)
INCLUDE ( [ScheduleId],
[Arrival],
[Departure],
[Pass]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrainStop_Origin]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrainStop] ADD  CONSTRAINT [DF_ScheduleTrainStop_Origin]  DEFAULT ((0)) FOR [Origin]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrainStop_Intermediate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrainStop] ADD  CONSTRAINT [DF_ScheduleTrainStop_Intermediate]  DEFAULT ((0)) FOR [Intermediate]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ScheduleTrainStop_Terminate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduleTrainStop] ADD  CONSTRAINT [DF_ScheduleTrainStop_Terminate]  DEFAULT ((0)) FOR [Terminate]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Station]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Station](
	[StationId] [smallint] IDENTITY(1,1) NOT NULL,
	[TiplocId] [smallint] NOT NULL,
	[StationName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[SubsiduaryAlphaCode] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Location] [geography] NULL,
 CONSTRAINT [PK_Station] PRIMARY KEY CLUSTERED 
(
	[StationId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Station]') AND name = N'''IX_Station_ByTiploc_Location''')
CREATE NONCLUSTERED INDEX ['IX_Station_ByTiploc_Location'] ON [dbo].[Station] 
(
	[TiplocId] ASC
)
INCLUDE ( [StationName],
[Location]) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TrainAssociation]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TrainAssociation](
	[AssociationId] [uniqueidentifier] NOT NULL,
	[MainTrainUid] [nvarchar](6) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[AssocTrainUid] [nvarchar](6) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	[AppliesMonday] [bit] NULL,
	[AppliesTuesday] [bit] NULL,
	[AppliesWednesday] [bit] NULL,
	[AppliesThursday] [bit] NULL,
	[AppliesFriday] [bit] NULL,
	[AppliesSaturday] [bit] NULL,
	[AppliesSunday] [bit] NULL,
	[LocationTiplocId] [smallint] NOT NULL,
	[AssociationDate] [tinyint] NOT NULL,
	[AssociationType] [tinyint] NOT NULL,
	[STPIndicatorId] [tinyint] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_TrainAssociation] PRIMARY KEY CLUSTERED 
(
	[AssociationId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_TrainAssociation_AssociationId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[TrainAssociation] ADD  CONSTRAINT [DF_TrainAssociation_AssociationId]  DEFAULT (newid()) FOR [AssociationId]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_TrainAssociation_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[TrainAssociation] ADD  CONSTRAINT [DF_TrainAssociation_Deleted]  DEFAULT ((0)) FOR [Deleted]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrain_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrain]'))
ALTER TABLE [dbo].[LiveTrain]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrain_Tiploc] FOREIGN KEY([OriginTiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrain_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrain]'))
ALTER TABLE [dbo].[LiveTrain] CHECK CONSTRAINT [FK_LiveTrain_Tiploc]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainCancellation_DelayAttributionCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainCancellation]'))
ALTER TABLE [dbo].[LiveTrainCancellation]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainCancellation_DelayAttributionCodes] FOREIGN KEY([ReasonCode])
REFERENCES [dbo].[DelayAttributionCodes] ([ReasonCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainCancellation_DelayAttributionCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainCancellation]'))
ALTER TABLE [dbo].[LiveTrainCancellation] CHECK CONSTRAINT [FK_LiveTrainCancellation_DelayAttributionCodes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainCancellation_LiveTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainCancellation]'))
ALTER TABLE [dbo].[LiveTrainCancellation]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainCancellation_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainCancellation_LiveTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainCancellation]'))
ALTER TABLE [dbo].[LiveTrainCancellation] CHECK CONSTRAINT [FK_LiveTrainCancellation_LiveTrain]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainCancellation_LiveTrainCancellation]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainCancellation]'))
ALTER TABLE [dbo].[LiveTrainCancellation]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainCancellation_LiveTrainCancellation] FOREIGN KEY([TrainCancellationId])
REFERENCES [dbo].[LiveTrainCancellation] ([TrainCancellationId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainCancellation_LiveTrainCancellation]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainCancellation]'))
ALTER TABLE [dbo].[LiveTrainCancellation] CHECK CONSTRAINT [FK_LiveTrainCancellation_LiveTrainCancellation]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainChangeOfOrigin_DelayAttributionCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainChangeOfOrigin]'))
ALTER TABLE [dbo].[LiveTrainChangeOfOrigin]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainChangeOfOrigin_DelayAttributionCodes] FOREIGN KEY([ReasonCode])
REFERENCES [dbo].[DelayAttributionCodes] ([ReasonCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainChangeOfOrigin_DelayAttributionCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainChangeOfOrigin]'))
ALTER TABLE [dbo].[LiveTrainChangeOfOrigin] CHECK CONSTRAINT [FK_LiveTrainChangeOfOrigin_DelayAttributionCodes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainChangeOfOrigin_LiveTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainChangeOfOrigin]'))
ALTER TABLE [dbo].[LiveTrainChangeOfOrigin]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainChangeOfOrigin_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainChangeOfOrigin_LiveTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainChangeOfOrigin]'))
ALTER TABLE [dbo].[LiveTrainChangeOfOrigin] CHECK CONSTRAINT [FK_LiveTrainChangeOfOrigin_LiveTrain]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainChangeOfOrigin_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainChangeOfOrigin]'))
ALTER TABLE [dbo].[LiveTrainChangeOfOrigin]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainChangeOfOrigin_Tiploc] FOREIGN KEY([NewTiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainChangeOfOrigin_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainChangeOfOrigin]'))
ALTER TABLE [dbo].[LiveTrainChangeOfOrigin] CHECK CONSTRAINT [FK_LiveTrainChangeOfOrigin_Tiploc]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainReinstatement_LiveTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainReinstatement]'))
ALTER TABLE [dbo].[LiveTrainReinstatement]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainReinstatement_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainReinstatement_LiveTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainReinstatement]'))
ALTER TABLE [dbo].[LiveTrainReinstatement] CHECK CONSTRAINT [FK_LiveTrainReinstatement_LiveTrain]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainReinstatement_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainReinstatement]'))
ALTER TABLE [dbo].[LiveTrainReinstatement]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainReinstatement_Tiploc] FOREIGN KEY([ReinstatedTiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainReinstatement_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainReinstatement]'))
ALTER TABLE [dbo].[LiveTrainReinstatement] CHECK CONSTRAINT [FK_LiveTrainReinstatement_Tiploc]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainStop_LiveTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainStop]'))
ALTER TABLE [dbo].[LiveTrainStop]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainStop_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainStop_LiveTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainStop]'))
ALTER TABLE [dbo].[LiveTrainStop] CHECK CONSTRAINT [FK_LiveTrainStop_LiveTrain]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainStop_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainStop]'))
ALTER TABLE [dbo].[LiveTrainStop]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainStop_Tiploc] FOREIGN KEY([ReportingTiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LiveTrainStop_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[LiveTrainStop]'))
ALTER TABLE [dbo].[LiveTrainStop] CHECK CONSTRAINT [FK_LiveTrainStop_Tiploc]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_AtocCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_AtocCode] FOREIGN KEY([AtocCode])
REFERENCES [dbo].[AtocCode] ([AtocCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_AtocCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_AtocCode]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_ScheduleSource]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_ScheduleSource] FOREIGN KEY([Source])
REFERENCES [dbo].[ScheduleSource] ([ScheduleSourceId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_ScheduleSource]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_ScheduleSource]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_ScheduleStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_ScheduleStatus] FOREIGN KEY([ScheduleStatusId])
REFERENCES [dbo].[ScheduleStatus] ([StatusId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_ScheduleStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_ScheduleStatus]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_ScheduleTrainCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_ScheduleTrainCategory] FOREIGN KEY([CategoryTypeId])
REFERENCES [dbo].[ScheduleTrainCategory] ([TrainCategoryId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_ScheduleTrainCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_ScheduleTrainCategory]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_ScheduleTrainPowerType]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_ScheduleTrainPowerType] FOREIGN KEY([PowerTypeId])
REFERENCES [dbo].[ScheduleTrainPowerType] ([PowerTypeId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_ScheduleTrainPowerType]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_ScheduleTrainPowerType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_STPIndicator]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_STPIndicator] FOREIGN KEY([STPIndicatorId])
REFERENCES [dbo].[STPIndicator] ([STPIndicatorId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_STPIndicator]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_STPIndicator]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_Tiploc] FOREIGN KEY([OriginStopTiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_Tiploc]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_Tiploc1]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_Tiploc1] FOREIGN KEY([DestinationStopTiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrain_Tiploc1]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrain]'))
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_Tiploc1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrainStop_ScheduleTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrainStop]'))
ALTER TABLE [dbo].[ScheduleTrainStop]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrainStop_ScheduleTrain] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[ScheduleTrain] ([ScheduleId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrainStop_ScheduleTrain]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrainStop]'))
ALTER TABLE [dbo].[ScheduleTrainStop] CHECK CONSTRAINT [FK_ScheduleTrainStop_ScheduleTrain]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrainStop_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrainStop]'))
ALTER TABLE [dbo].[ScheduleTrainStop]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrainStop_Tiploc] FOREIGN KEY([TiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ScheduleTrainStop_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[ScheduleTrainStop]'))
ALTER TABLE [dbo].[ScheduleTrainStop] CHECK CONSTRAINT [FK_ScheduleTrainStop_Tiploc]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Station_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[Station]'))
ALTER TABLE [dbo].[Station]  WITH CHECK ADD  CONSTRAINT [FK_Station_Tiploc] FOREIGN KEY([TiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Station_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[Station]'))
ALTER TABLE [dbo].[Station] CHECK CONSTRAINT [FK_Station_Tiploc]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TrainAssociation_STPIndicator]') AND parent_object_id = OBJECT_ID(N'[dbo].[TrainAssociation]'))
ALTER TABLE [dbo].[TrainAssociation]  WITH CHECK ADD  CONSTRAINT [FK_TrainAssociation_STPIndicator] FOREIGN KEY([STPIndicatorId])
REFERENCES [dbo].[STPIndicator] ([STPIndicatorId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TrainAssociation_STPIndicator]') AND parent_object_id = OBJECT_ID(N'[dbo].[TrainAssociation]'))
ALTER TABLE [dbo].[TrainAssociation] CHECK CONSTRAINT [FK_TrainAssociation_STPIndicator]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TrainAssociation_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[TrainAssociation]'))
ALTER TABLE [dbo].[TrainAssociation]  WITH CHECK ADD  CONSTRAINT [FK_TrainAssociation_Tiploc] FOREIGN KEY([LocationTiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TrainAssociation_Tiploc]') AND parent_object_id = OBJECT_ID(N'[dbo].[TrainAssociation]'))
ALTER TABLE [dbo].[TrainAssociation] CHECK CONSTRAINT [FK_TrainAssociation_Tiploc]
GO
