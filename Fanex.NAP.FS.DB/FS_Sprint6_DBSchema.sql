/*
Creator: 20180503@LongLuu
Task:  Create tables for Fraud Sensor - Sprint#6
Server:  bodb_FraudSensor

Reviewer: Billy
*/ 

USE	bodb_FraudSensor;
GO

--=========== CREATE TABLES
CREATE TABLE [dbo].[TicketsByIP](
	[TransId]				BIGINT NOT NULL,
	[IP]					NVARCHAR(15) NOT NULL
CONSTRAINT [PK_TicketsByIP] PRIMARY KEY CLUSTERED 
(
	[TransId]				ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]
GO

CREATE CLUSTERED INDEX CIX_TicketsByIP_IP ON [dbo].[TicketsByIP] (
	[IP]					ASC
) ON IDX;
GO

--=========== ALTER TABLES
DROP  TABLE [dbo].[CustomerLogType];
GO

