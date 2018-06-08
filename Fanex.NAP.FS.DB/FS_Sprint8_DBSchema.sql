/*
Creator: 20180605@LongLuu
Task:  Create tables for Fraud Sensor - Sprint#8
Server:  bodb_FraudSensor

Reviewer: Billy
*/ 

USE	bodb_FraudSensor;
GO


----===================A. DROP TABLES=========================
----==========================================================



----===================B. CREATE TABLES========================
----===========================================================
--*** @CaseyHuynh: Create Table OfficeIP
CREATE TABLE	dbo.OfficeIP(
	IPRange	NVARCHAR(11) NOT NULL
	,FromNumber		SMALLINT NOT NULL
	,ToNumber		SMALLINT NOT NULL		
CONSTRAINT PK_OfficeIP_IPRangeFromNumberToNumber PRIMARY KEY CLUSTERED 
(
	IPRange			ASC
	,FromNumber		ASC
	,ToNumber		ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON DATA
) ON DATA
GO

CREATE NONCLUSTERED INDEX NIX_OfficeIP_IPRange ON dbo.OfficeIP (
	IPRange		ASC
) ON IDX;

CREATE NONCLUSTERED INDEX NIX_OfficeIP_FromNumberToNumber ON dbo.OfficeIP (
	FromNumber		ASC
	,ToNumber		ASC
) ON IDX;
GO

CREATE TABLE [dbo].[FraudNotificationRecipient](
	[FraudCaseId]		TINYINT NOT NULL,
	[RecipientId]		INT NOT NULL
 CONSTRAINT [PK_FraudCaseRecipient] PRIMARY KEY CLUSTERED 
(
	[FraudCaseId]		ASC,
	[RecipientId]		ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]
GO

CREATE TABLE [dbo].[Message](
	[MessageId]			INT IDENTITY(1,1) NOT NULL,
	[FraudCaseId]		TINYINT NOT NULL,
	[MessageContent]	NVARCHAR(1000) NULL,
	[LogDate]			DATETIME NULL
 CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]

CREATE NONCLUSTERED INDEX NCIX_Message_FraudCaseId ON [dbo].[Message] (
	[FraudCaseId]		ASC
) ON IDX;
GO

CREATE TABLE [dbo].[UserMessage](
	[UserMessageId]		INT IDENTITY(1,1) NOT NULL,
	[UserId]			INT NOT NULL,
	[MessageId]			INT NOT NULL,
	[IsViewed]			BIT NOT NULL
 CONSTRAINT [PK_UserMessage] PRIMARY KEY CLUSTERED 
(
	[UserMessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]
GO

CREATE NONCLUSTERED INDEX NCIX_UserMessage_UserIdMessageId ON [dbo].[UserMessage] (
	[UserId]			ASC,
	[MessageId]			ASC,
	[IsViewed]			ASC
) ON IDX;
GO

CREATE TABLE [dbo].[CapturedDataType](
	[DataTypeId]		INT NOT NULL,
	[DataName]			NVARCHAR(100) NULL,
 CONSTRAINT [PK_CapturedDataType] PRIMARY KEY CLUSTERED 
(
	[DataTypeId]		ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]
GO

CREATE TABLE [dbo].[CapturedData](
	[MatchId]					INT NOT NULL,
	[TotalCompanyTO]			MONEY NULL,
	[TotalSuperTO]				MONEY NULL,
	[TotalMasterTO]				MONEY NULL,
	[TotalAgentTO]				MONEY NULL,
	[TotalTickets]				INT NULL,
	[TotalSettledTickets]		INT NULL,
	[TotalCancelledTickets]		INT NULL,
	[CapturedDate]				DATETIME NOT NULL,
 CONSTRAINT [PK_CapturedData] PRIMARY KEY CLUSTERED 
(
	[MatchId]					ASC,
	[CapturedDate]				ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]
GO

CREATE TABLE [dbo].[ChangedMatch](
	[MatchId]						INT NOT NULL,
	[OldTotalCompanyTO]				MONEY NULL,
	[NewTotalCompanyTO]				MONEY NULL,
	[OldTotalSuperTO]				MONEY NULL,
	[NewTotalSuperTO]				MONEY NULL,
	[OldTotalMasterTO]				MONEY NULL,
	[NewTotalMasterTO]				MONEY NULL,
	[OldTotalAgentTO]				MONEY NULL,
	[NewTotalAgentTO]				MONEY NULL,
	[OldTotalTickets]				INT NULL,
	[NewTotalTickets]				INT NULL,
	[OldTotalSettledTickets]		INT NULL,
	[NewTotalSettledTickets]		INT NULL,
	[OldTotalCancelledTickets]		INT NULL,
	[NewTotalCancelledTickets]		INT NULL,
	[CapturedDate]					DATETIME NOT NULL
 CONSTRAINT [PK_ChangedMatch] PRIMARY KEY CLUSTERED 
(
	[MatchId]						ASC,
	[CapturedDate]					ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]
GO
--================================================
CREATE TABLE dbo.BSNGMatchScanning
(
	[MatchId]					INT	NOT NULL
	, [LiveHomeScore]			TINYINT NOT NULL
	, [LiveAwayScore]			TINYINT NOT NULL
	, [EventDate]				DATETIME NOT NULL
	, [EventStatus]				NVARCHAR(50) NOT NULL
	, [LastRejectedTicket]		BIGINT NULL
	, [LastRejectedTime]		DATETIME NULL
	, [ScannedTime]			DATETIME NULL
	, [CreatedTime]			DATETIME NULL
 CONSTRAINT [PK_BSNGMatchScanning_MatchIdLiveHomeScoreLiveAwayScore] PRIMARY KEY CLUSTERED 
(
	 MatchId			ASC
	, LiveHomeScore		ASC
	, LiveAwayScore		ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA];

GO
CREATE NONCLUSTERED INDEX NCIX_BSNGScanInfo_EventDate ON [dbo].[BSNGMatchScanning] (
	[EventDate]			ASC
) ON IDX;
GO
--================================================ DROP TABLE BSNGMatchScanningArchive
CREATE TABLE dbo.BSNGMatchScanningArchive
(
	[MatchId]					INT	NOT NULL
	, [LiveHomeScore]			TINYINT NOT NULL
	, [LiveAwayScore]			TINYINT NOT NULL
	, [EventDate]				DATETIME NOT NULL
	, [EventStatus]				NVARCHAR(50) NOT NULL
	, [LastRejectedTicket]		BIGINT NULL
	, [LastRejectedTime]		DATETIME NULL
	, [ScannedTime]			DATETIME NULL
	, [CreatedTime]			DATETIME NULL
 CONSTRAINT [PK_BSNGMatchScanningArchive_MatchIdLiveHomeScoreLiveAwayScore] PRIMARY KEY CLUSTERED 
(
	 MatchId			ASC
	, LiveHomeScore		ASC
	, LiveAwayScore		ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA];
GO
CREATE NONCLUSTERED INDEX NCIX_BSNGScanInfo_EventDate ON [dbo].[BSNGMatchScanningArchive] (
	[EventDate]			ASC
) ON IDX;
GO

----===================C. ALTER TABLES=========================
----===========================================================
