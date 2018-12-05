/*
Creator: 20181113@LongLuu
Task:  Permission for Special Currencies - [Redmine ID: 104707]
Server:  bodb_FraudSensor

Reviewer: Billy
*/ 

USE	bodb_FraudSensor;
GO


----===================A. DROP TABLES=========================
----==========================================================



----===================B. CREATE TABLES========================
----===========================================================
CREATE TABLE [dbo].[SpecialCurrency](
	[CurrencyId]			INT NOT NULL,
	[CurrencyName]			NVARCHAR(100) NULL,
	[IsTest]				BIT DEFAULT(0),
CONSTRAINT [PK_SpecialCurrency] PRIMARY KEY CLUSTERED 
(
	[CurrencyId]			ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]
GO

INSERT INTO dbo.SpecialCurrency(CurrencyId, CurrencyName, IsTest)
VALUES(2, 'RM', 0);
INSERT INTO dbo.SpecialCurrency(CurrencyId, CurrencyName, IsTest)
VALUES(3, 'US$', 0);

----===================A. ALTER TABLES=========================
----==========================================================+