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

----===================C. ALTER TABLES=========================
----===========================================================

----===================D. INIT DATA=========================
----==========================================================
INSERT INTO dbo.FraudCase(FraudCaseName, FraudCaseDesc, FraudCaseInterval, IsActive, IsTest, IsInternal)
VALUES('Bets Near Goal', 'Bets Near Goal', 60, 1, 1, 1);