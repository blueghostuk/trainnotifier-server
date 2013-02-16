USE [natrail]
GO
/****** Object:  Table [dbo].[Tiploc]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tiploc](
	[TiplocId] [smallint] IDENTITY(1,1) NOT NULL,
	[Tiploc] [nvarchar](7) NOT NULL,
	[Nalco] [nvarchar](6) NULL,
	[Description] [nvarchar](26) NULL,
	[Stanox] [nvarchar](5) NULL,
	[CRS] [nvarchar](3) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Tiploc] PRIMARY KEY CLUSTERED 
(
	[TiplocId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TDArea]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TDArea](
	[TDAreaId] [nvarchar](2) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TDArea] PRIMARY KEY CLUSTERED 
(
	[TDAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[STPIndicator]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STPIndicator](
	[STPIndicatorId] [tinyint] NOT NULL,
	[Code] [nvarchar](2) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_STPIndicator] PRIMARY KEY CLUSTERED 
(
	[STPIndicatorId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveTrain]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LiveTrain](
	[Id] [uniqueidentifier] NOT NULL,
	[TrainId] [nvarchar](max) NOT NULL,
	[Headcode] [nvarchar](4) NOT NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[OriginDepartTimestamp] [datetime] NOT NULL,
	[TrainServiceCode] [nvarchar](max) NULL,
	[Toc] [nvarchar](3) NOT NULL,
	[TrainUid] [nvarchar](max) NOT NULL,
	[OriginStanox] [nvarchar](5) NOT NULL,
	[SchedWttId] [nvarchar](10) NOT NULL,
	[Activated] [bit] NOT NULL,
	[Cancelled] [bit] NOT NULL,
	[Terminated] [bit] NOT NULL,
 CONSTRAINT [PK_LiveTrain] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AtocCode]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AtocCode](
	[AtocCode] [nvarchar](2) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AtocCode] PRIMARY KEY CLUSTERED 
(
	[AtocCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleStatus]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleStatus](
	[StatusId] [tinyint] NOT NULL,
	[Code] [nvarchar](2) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ScheduleStatus] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleTrain]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleTrain](
	[ScheduleId] [uniqueidentifier] NOT NULL,
	[TrainUid] [nvarchar](6) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	[Active] [bit] NOT NULL,
	[AtocCode] [nvarchar](2) NULL,
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
 CONSTRAINT [PK_ScheduleTrain] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveTrainStop]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LiveTrainStop](
	[TrainStopId] [uniqueidentifier] NOT NULL,
	[TrainId] [uniqueidentifier] NOT NULL,
	[EventType] [nvarchar](50) NOT NULL,
	[PlannedTimestamp] [datetime] NULL,
	[ActualTimestamp] [datetime] NOT NULL,
	[ReportingStanox] [nvarchar](5) NOT NULL,
	[Platform] [nvarchar](50) NULL,
	[Line] [nvarchar](50) NULL,
	[TrainTerminated] [bit] NOT NULL,
 CONSTRAINT [PK_LiveTrainStop] PRIMARY KEY CLUSTERED 
(
	[TrainStopId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveTrainCancellation]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LiveTrainCancellation](
	[TrainCancellationId] [uniqueidentifier] NOT NULL,
	[TrainId] [uniqueidentifier] NOT NULL,
	[CancelledTimestamp] [datetime] NOT NULL,
	[Stanox] [nvarchar](5) NULL,
	[ReasonCode] [nvarchar](50) NULL,
	[Type] [nvarchar](50) NULL,
 CONSTRAINT [PK_LiveTrainCancellation] PRIMARY KEY CLUSTERED 
(
	[TrainCancellationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveTrainBerth]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LiveTrainBerth](
	[TrainBerthId] [uniqueidentifier] NOT NULL,
	[TrainId] [uniqueidentifier] NOT NULL,
	[MessageType] [nvarchar](5) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[AreaId] [nvarchar](2) NOT NULL,
	[From] [nvarchar](50) NULL,
	[To] [nvarchar](50) NULL,
	[ReportTime] [nvarchar](50) NULL,
 CONSTRAINT [PK_LiveTrainBerth] PRIMARY KEY CLUSTERED 
(
	[TrainBerthId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Station]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Station](
	[StationId] [smallint] IDENTITY(1,1) NOT NULL,
	[TiplocId] [smallint] NOT NULL,
	[StationName] [nvarchar](30) NOT NULL,
	[SubsiduaryAlphaCode] [nvarchar](3) NULL,
	[AlphaCode] [nvarchar](3) NOT NULL,
	[Location] [geography] NULL,
	[LocationEstimated] [bit] NOT NULL,
 CONSTRAINT [PK_Station] PRIMARY KEY CLUSTERED 
(
	[StationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleTrainStop]    Script Date: 02/16/2013 17:12:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleTrainStop](
	[ScheduleId] [uniqueidentifier] NOT NULL,
	[StopNumber] [tinyint] NOT NULL,
	[TiplocId] [smallint] NOT NULL,
	[Arrival] [time](0) NULL,
	[Departure] [time](0) NULL,
	[Pass] [time](0) NULL,
	[PublicArrival] [time](0) NULL,
	[PublicDeparture] [time](0) NULL,
	[Line] [nchar](10) NULL,
	[Path] [nchar](10) NULL,
	[Platform] [nvarchar](50) NULL,
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Default [DF_LiveTrain_Id]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrain] ADD  CONSTRAINT [DF_LiveTrain_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
/****** Object:  Default [DF_LiveTrain_Activated]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrain] ADD  CONSTRAINT [DF_LiveTrain_Activated]  DEFAULT ((1)) FOR [Activated]
GO
/****** Object:  Default [DF_LiveTrain_Cancelled]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrain] ADD  CONSTRAINT [DF_LiveTrain_Cancelled]  DEFAULT ((0)) FOR [Cancelled]
GO
/****** Object:  Default [DF_LiveTrain_Terminated]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrain] ADD  CONSTRAINT [DF_LiveTrain_Terminated]  DEFAULT ((0)) FOR [Terminated]
GO
/****** Object:  Default [DF_LiveTrainBerth_TrainBerthId]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrainBerth] ADD  CONSTRAINT [DF_LiveTrainBerth_TrainBerthId]  DEFAULT (newsequentialid()) FOR [TrainBerthId]
GO
/****** Object:  Default [DF_LiveTrainCancellation_TrainCancellationId]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrainCancellation] ADD  CONSTRAINT [DF_LiveTrainCancellation_TrainCancellationId]  DEFAULT (newsequentialid()) FOR [TrainCancellationId]
GO
/****** Object:  Default [DF_LiveTrainStop_TrainStopId]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrainStop] ADD  CONSTRAINT [DF_LiveTrainStop_TrainStopId]  DEFAULT (newsequentialid()) FOR [TrainStopId]
GO
/****** Object:  Default [DF_LiveTrainStop_TrainTerminated]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrainStop] ADD  CONSTRAINT [DF_LiveTrainStop_TrainTerminated]  DEFAULT ((0)) FOR [TrainTerminated]
GO
/****** Object:  Default [DF_ScheduleTrain_ScheduleId]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_ScheduleId]  DEFAULT (newsequentialid()) FOR [ScheduleId]
GO
/****** Object:  Default [DF_ScheduleTrain_Active]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_Active]  DEFAULT ((1)) FOR [Active]
GO
/****** Object:  Default [DF_ScheduleTrain_RunsMonday]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsMonday]  DEFAULT ((0)) FOR [RunsMonday]
GO
/****** Object:  Default [DF_ScheduleTrain_RunsTuesday]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsTuesday]  DEFAULT ((0)) FOR [RunsTuesday]
GO
/****** Object:  Default [DF_ScheduleTrain_RunsWednesday]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsWednesday]  DEFAULT ((0)) FOR [RunsWednesday]
GO
/****** Object:  Default [DF_ScheduleTrain_RunsThursday]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsThursday]  DEFAULT ((0)) FOR [RunsThursday]
GO
/****** Object:  Default [DF_ScheduleTrain_RunsFriday]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsFriday]  DEFAULT ((0)) FOR [RunsFriday]
GO
/****** Object:  Default [DF_ScheduleTrain_RunsSaturday]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsSaturday]  DEFAULT ((0)) FOR [RunsSaturday]
GO
/****** Object:  Default [DF_ScheduleTrain_RunsSunday]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsSunday]  DEFAULT ((0)) FOR [RunsSunday]
GO
/****** Object:  Default [DF_ScheduleTrain_RunsBankHoliday]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain] ADD  CONSTRAINT [DF_ScheduleTrain_RunsBankHoliday]  DEFAULT ((0)) FOR [RunsBankHoliday]
GO
/****** Object:  Default [DF_ScheduleTrainStop_Origin]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrainStop] ADD  CONSTRAINT [DF_ScheduleTrainStop_Origin]  DEFAULT ((0)) FOR [Origin]
GO
/****** Object:  Default [DF_ScheduleTrainStop_Intermediate]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrainStop] ADD  CONSTRAINT [DF_ScheduleTrainStop_Intermediate]  DEFAULT ((0)) FOR [Intermediate]
GO
/****** Object:  Default [DF_ScheduleTrainStop_Terminate]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrainStop] ADD  CONSTRAINT [DF_ScheduleTrainStop_Terminate]  DEFAULT ((0)) FOR [Terminate]
GO
/****** Object:  Default [DF_Tiploc_Deleted]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[Tiploc] ADD  CONSTRAINT [DF_Tiploc_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO
/****** Object:  ForeignKey [FK_LiveTrainBerth_LiveTrain]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrainBerth]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainBerth_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
ALTER TABLE [dbo].[LiveTrainBerth] CHECK CONSTRAINT [FK_LiveTrainBerth_LiveTrain]
GO
/****** Object:  ForeignKey [FK_LiveTrainBerth_TDArea]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrainBerth]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainBerth_TDArea] FOREIGN KEY([AreaId])
REFERENCES [dbo].[TDArea] ([TDAreaId])
GO
ALTER TABLE [dbo].[LiveTrainBerth] CHECK CONSTRAINT [FK_LiveTrainBerth_TDArea]
GO
/****** Object:  ForeignKey [FK_LiveTrainCancellation_LiveTrain]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrainCancellation]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainCancellation_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
ALTER TABLE [dbo].[LiveTrainCancellation] CHECK CONSTRAINT [FK_LiveTrainCancellation_LiveTrain]
GO
/****** Object:  ForeignKey [FK_LiveTrainStop_LiveTrain]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[LiveTrainStop]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainStop_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
ALTER TABLE [dbo].[LiveTrainStop] CHECK CONSTRAINT [FK_LiveTrainStop_LiveTrain]
GO
/****** Object:  ForeignKey [FK_ScheduleTrain_AtocCode]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_AtocCode] FOREIGN KEY([AtocCode])
REFERENCES [dbo].[AtocCode] ([AtocCode])
GO
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_AtocCode]
GO
/****** Object:  ForeignKey [FK_ScheduleTrain_ScheduleStatus]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_ScheduleStatus] FOREIGN KEY([ScheduleStatusId])
REFERENCES [dbo].[ScheduleStatus] ([StatusId])
GO
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_ScheduleStatus]
GO
/****** Object:  ForeignKey [FK_ScheduleTrain_STPIndicator]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_STPIndicator] FOREIGN KEY([STPIndicatorId])
REFERENCES [dbo].[STPIndicator] ([STPIndicatorId])
GO
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_STPIndicator]
GO
/****** Object:  ForeignKey [FK_ScheduleTrain_Tiploc]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_Tiploc] FOREIGN KEY([OriginStopTiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_Tiploc]
GO
/****** Object:  ForeignKey [FK_ScheduleTrain_Tiploc1]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrain]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrain_Tiploc1] FOREIGN KEY([DestinationStopTiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
ALTER TABLE [dbo].[ScheduleTrain] CHECK CONSTRAINT [FK_ScheduleTrain_Tiploc1]
GO
/****** Object:  ForeignKey [FK_ScheduleTrainStop_ScheduleTrain]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrainStop]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrainStop_ScheduleTrain] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[ScheduleTrain] ([ScheduleId])
GO
ALTER TABLE [dbo].[ScheduleTrainStop] CHECK CONSTRAINT [FK_ScheduleTrainStop_ScheduleTrain]
GO
/****** Object:  ForeignKey [FK_ScheduleTrainStop_Tiploc]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[ScheduleTrainStop]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTrainStop_Tiploc] FOREIGN KEY([TiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
ALTER TABLE [dbo].[ScheduleTrainStop] CHECK CONSTRAINT [FK_ScheduleTrainStop_Tiploc]
GO
/****** Object:  ForeignKey [FK_Station_Tiploc]    Script Date: 02/16/2013 17:12:08 ******/
ALTER TABLE [dbo].[Station]  WITH CHECK ADD  CONSTRAINT [FK_Station_Tiploc] FOREIGN KEY([TiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
ALTER TABLE [dbo].[Station] CHECK CONSTRAINT [FK_Station_Tiploc]
GO
