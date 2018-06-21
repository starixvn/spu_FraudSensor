/*
Creator: 20180618@LongLuu
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
CREATE TABLE [dbo].[CheckBetCategory](
	[CategoryId]			INT NOT NULL,
	[CategoryName]			NVARCHAR(100) NULL,
	[CategoryDataType]		NVARCHAR(15) NULL,
	[IsTest]				BIT DEFAULT(0),
CONSTRAINT [PK_CheckBetCategory] PRIMARY KEY CLUSTERED 
(
	[CategoryId]			ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]
GO

CREATE TABLE [dbo].[ChangedTicket](
	[CategoryId]			INT NOT NULL,
	[TransId]				BIGINT NOT NULL,
	[CapturedDate]			DATETIME NOT NULL,
	[NewValue]				NVARCHAR(20) NULL,
	[OldValue]				NVARCHAR(20) NULL,
	[OldCaptureDate]		DATETIME NULL,
CONSTRAINT [PK_ChangedTicket] PRIMARY KEY CLUSTERED 
(
	[CategoryId]			ASC,
	[TransId]				ASC,
	[CapturedDate]			ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]
GO
----===================C. ALTER TABLES=========================
----===========================================================
