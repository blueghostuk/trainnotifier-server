USE [master]
GO
/****** Object:  Database [natrail]    Script Date: 11/16/2012 07:49:53 ******/
CREATE DATABASE [natrail] ON  PRIMARY 
( NAME = N'natrail', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\natrail.mdf' , SIZE = 216064KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'natrail_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\natrail_log.ldf' , SIZE = 102144KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [natrail] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [natrail].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [natrail] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [natrail] SET ANSI_NULLS OFF
GO
ALTER DATABASE [natrail] SET ANSI_PADDING OFF
GO
ALTER DATABASE [natrail] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [natrail] SET ARITHABORT OFF
GO
ALTER DATABASE [natrail] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [natrail] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [natrail] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [natrail] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [natrail] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [natrail] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [natrail] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [natrail] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [natrail] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [natrail] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [natrail] SET  DISABLE_BROKER
GO
ALTER DATABASE [natrail] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [natrail] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [natrail] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [natrail] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [natrail] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [natrail] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [natrail] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [natrail] SET  READ_WRITE
GO
ALTER DATABASE [natrail] SET RECOVERY FULL
GO
ALTER DATABASE [natrail] SET  MULTI_USER
GO
ALTER DATABASE [natrail] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [natrail] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'natrail', N'ON'
GO
USE [natrail]
GO
/****** Object:  User [natrail]    Script Date: 11/16/2012 07:49:54 ******/
CREATE USER [natrail] FOR LOGIN [natrail] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[Tiploc]    Script Date: 11/16/2012 07:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tiploc](
	[TiplocId] [smallint] NOT NULL,
	[Tiploc] [nvarchar](7) NOT NULL,
	[Nalco] [nvarchar](6) NULL,
	[Description] [nvarchar](26) NULL,
	[Stanox] [nvarchar](5) NULL,
	[CRS] [nvarchar](3) NULL,
	[Deleted] [bit] NOT NULL,
	[DateDeleted] [datetime] NULL,
 CONSTRAINT [PK_Tiploc] PRIMARY KEY CLUSTERED 
(
	[TiplocId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tiploc] ON [dbo].[Tiploc] 
(
	[Tiploc] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TDArea]    Script Date: 11/16/2012 07:49:58 ******/
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
/****** Object:  Table [dbo].[LiveTrainState]    Script Date: 11/16/2012 07:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LiveTrainState](
	[StateId] [tinyint] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_LiveTrainState] PRIMARY KEY CLUSTERED 
(
	[StateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveTrain]    Script Date: 11/16/2012 07:49:58 ******/
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
	[StateId] [tinyint] NOT NULL,
 CONSTRAINT [PK_LiveTrain] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LiveTrain_Headcode_StateId] ON [dbo].[LiveTrain] 
(
	[Headcode] ASC,
	[StateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LiveTrain_OriginStanox] ON [dbo].[LiveTrain] 
(
	[OriginStanox] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LiveTrain_SchedWttId] ON [dbo].[LiveTrain] 
(
	[SchedWttId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Station]    Script Date: 11/16/2012 07:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Station](
	[StationId] [smallint] IDENTITY(1,1) NOT NULL,
	[TiplocId] [smallint] NOT NULL,
	[StationName] [nvarchar](30) NOT NULL,
	[CATEType] [tinyint] NOT NULL,
	[SubsiduaryAlphaCode] [nvarchar](3) NULL,
	[AlphaCode] [nvarchar](3) NOT NULL,
	[Location] [geometry] NULL,
	[LocationEstimated] [bit] NOT NULL,
	[ChangeTime] [datetime] NULL,
	[ManualEntry] [bit] NOT NULL,
 CONSTRAINT [PK_Station] PRIMARY KEY CLUSTERED 
(
	[StationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveTrainStop]    Script Date: 11/16/2012 07:49:58 ******/
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
CREATE NONCLUSTERED INDEX [IX_LiveTrainStop_ReportingStanox] ON [dbo].[LiveTrainStop] 
(
	[ReportingStanox] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LiveTrainStop_ReportingStanox_ActualTimeStamp] ON [dbo].[LiveTrainStop] 
(
	[ReportingStanox] ASC,
	[ActualTimestamp] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveTrainCancellation]    Script Date: 11/16/2012 07:49:58 ******/
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
CREATE NONCLUSTERED INDEX [IX_LiveTrainCancellation_Stanox] ON [dbo].[LiveTrainCancellation] 
(
	[Stanox] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveTrainBerth]    Script Date: 11/16/2012 07:49:58 ******/
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
/****** Object:  Default [DF_Tiploc_Deleted]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[Tiploc] ADD  CONSTRAINT [DF_Tiploc_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO
/****** Object:  Default [DF_LiveTrain_Id]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrain] ADD  CONSTRAINT [DF_LiveTrain_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
/****** Object:  Default [DF_LiveTrain_StateUd]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrain] ADD  CONSTRAINT [DF_LiveTrain_StateUd]  DEFAULT ((4)) FOR [StateId]
GO
/****** Object:  Default [DF_Station_ManualEntry]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[Station] ADD  CONSTRAINT [DF_Station_ManualEntry]  DEFAULT ((0)) FOR [ManualEntry]
GO
/****** Object:  Default [DF_LiveTrainStop_TrainStopId]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrainStop] ADD  CONSTRAINT [DF_LiveTrainStop_TrainStopId]  DEFAULT (newsequentialid()) FOR [TrainStopId]
GO
/****** Object:  Default [DF_LiveTrainStop_TrainTerminated]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrainStop] ADD  CONSTRAINT [DF_LiveTrainStop_TrainTerminated]  DEFAULT ((0)) FOR [TrainTerminated]
GO
/****** Object:  Default [DF_LiveTrainCancellation_TrainCancellationId]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrainCancellation] ADD  CONSTRAINT [DF_LiveTrainCancellation_TrainCancellationId]  DEFAULT (newsequentialid()) FOR [TrainCancellationId]
GO
/****** Object:  Default [DF_LiveTrainBerth_TrainBerthId]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrainBerth] ADD  CONSTRAINT [DF_LiveTrainBerth_TrainBerthId]  DEFAULT (newsequentialid()) FOR [TrainBerthId]
GO
/****** Object:  ForeignKey [FK_LiveTrain_LiveTrainState]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrain]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrain_LiveTrainState] FOREIGN KEY([StateId])
REFERENCES [dbo].[LiveTrainState] ([StateId])
GO
ALTER TABLE [dbo].[LiveTrain] CHECK CONSTRAINT [FK_LiveTrain_LiveTrainState]
GO
/****** Object:  ForeignKey [FK_Station_Tiploc]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[Station]  WITH CHECK ADD  CONSTRAINT [FK_Station_Tiploc] FOREIGN KEY([TiplocId])
REFERENCES [dbo].[Tiploc] ([TiplocId])
GO
ALTER TABLE [dbo].[Station] CHECK CONSTRAINT [FK_Station_Tiploc]
GO
/****** Object:  ForeignKey [FK_LiveTrainStop_LiveTrain]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrainStop]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainStop_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
ALTER TABLE [dbo].[LiveTrainStop] CHECK CONSTRAINT [FK_LiveTrainStop_LiveTrain]
GO
/****** Object:  ForeignKey [FK_LiveTrainCancellation_LiveTrain]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrainCancellation]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainCancellation_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
ALTER TABLE [dbo].[LiveTrainCancellation] CHECK CONSTRAINT [FK_LiveTrainCancellation_LiveTrain]
GO
/****** Object:  ForeignKey [FK_LiveTrainBerth_LiveTrain]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrainBerth]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainBerth_LiveTrain] FOREIGN KEY([TrainId])
REFERENCES [dbo].[LiveTrain] ([Id])
GO
ALTER TABLE [dbo].[LiveTrainBerth] CHECK CONSTRAINT [FK_LiveTrainBerth_LiveTrain]
GO
/****** Object:  ForeignKey [FK_LiveTrainBerth_TDArea]    Script Date: 11/16/2012 07:49:58 ******/
ALTER TABLE [dbo].[LiveTrainBerth]  WITH CHECK ADD  CONSTRAINT [FK_LiveTrainBerth_TDArea] FOREIGN KEY([AreaId])
REFERENCES [dbo].[TDArea] ([TDAreaId])
GO
ALTER TABLE [dbo].[LiveTrainBerth] CHECK CONSTRAINT [FK_LiveTrainBerth_TDArea]
GO
