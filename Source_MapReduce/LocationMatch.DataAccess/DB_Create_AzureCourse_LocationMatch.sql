USE [master]
GO
/****** Object:  Database [AzureCourse_LocationMatch]    Script Date: 11/28/2014 18:48:46 ******/
CREATE DATABASE [AzureCourse_LocationMatch] 
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AzureCourse_LocationMatch].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET ANSI_NULLS OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET ANSI_PADDING OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET ARITHABORT OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET  DISABLE_BROKER
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET  READ_WRITE
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET RECOVERY SIMPLE
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET  MULTI_USER
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [AzureCourse_LocationMatch] SET DB_CHAINING OFF
GO
USE [AzureCourse_LocationMatch]
GO
/****** Object:  Table [dbo].[Track]    Script Date: 11/28/2014 18:48:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Track](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Gpx] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Track] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Track_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationMatchAnalysis]    Script Date: 11/28/2014 18:48:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationMatchAnalysis](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LocationListId] [int] NOT NULL,
	[TrackId] [int] NOT NULL,
	[LocationListName] [nvarchar](50) NOT NULL,
	[TrackName] [nvarchar](50) NOT NULL,
	[Radius] [decimal](18, 2) NOT NULL,
	[TimeSubmitted] [datetime] NOT NULL,
	[TimeStarted] [datetime] NULL,
	[TimeFinished] [datetime] NULL,
	[Status] [nvarchar](10) NOT NULL,
	[Result] [nvarchar](max) NULL,
 CONSTRAINT [PK_AnalysisResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationList]    Script Date: 11/28/2014 18:48:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_LocationList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__LocationList_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Location]    Script Date: 11/28/2014 18:48:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Location](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Latitude] [numeric](9, 6) NOT NULL,
	[Longitude] [numeric](9, 6) NOT NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Location_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[B_LocationListLocation]    Script Date: 11/28/2014 18:48:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[B_LocationListLocation](
	[LocationListId] [int] NOT NULL,
	[LocationId] [int] NOT NULL,
 CONSTRAINT [PK_B_LocationListIdLocationId] PRIMARY KEY CLUSTERED 
(
	[LocationListId] ASC,
	[LocationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_B_LocationListLocation_B_LocationListLocation]    Script Date: 11/28/2014 18:48:48 ******/
ALTER TABLE [dbo].[B_LocationListLocation]  WITH CHECK ADD  CONSTRAINT [FK_B_LocationListLocation_B_LocationListLocation] FOREIGN KEY([LocationListId])
REFERENCES [dbo].[LocationList] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[B_LocationListLocation] CHECK CONSTRAINT [FK_B_LocationListLocation_B_LocationListLocation]
GO
/****** Object:  ForeignKey [FK_B_LocationListLocation_Location]    Script Date: 11/28/2014 18:48:48 ******/
ALTER TABLE [dbo].[B_LocationListLocation]  WITH CHECK ADD  CONSTRAINT [FK_B_LocationListLocation_Location] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Location] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[B_LocationListLocation] CHECK CONSTRAINT [FK_B_LocationListLocation_Location]
GO
