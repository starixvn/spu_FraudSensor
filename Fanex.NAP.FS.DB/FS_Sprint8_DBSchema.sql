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
CONSTRAINT PK_OfficeIP_IPRangeFromNumber PRIMARY KEY CLUSTERED 
(
	IPRange			ASC
	,FromNumber		ASC
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
	[UserId]			INT NOT NULL
 CONSTRAINT [PK_FraudCaseRecipient] PRIMARY KEY CLUSTERED 
(
	[FraudCaseId]		ASC,
	[UserId]			ASC
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
GO

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


----===================C. ALTER TABLES=========================
----===========================================================
