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
VALUES(1, 'SD$', 0);
INSERT INTO dbo.SpecialCurrency(CurrencyId, CurrencyName, IsTest)
VALUES(2, 'RM', 0);
INSERT INTO dbo.SpecialCurrency(CurrencyId, CurrencyName, IsTest)
VALUES(3, 'US$', 0);
INSERT INTO dbo.SpecialCurrency(CurrencyId, CurrencyName, IsTest)
VALUES(4, 'TB', 0);
INSERT INTO dbo.SpecialCurrency(CurrencyId, CurrencyName, IsTest)
VALUES(5, 'HK$', 0);
INSERT INTO dbo.SpecialCurrency(CurrencyId, CurrencyName, IsTest)
VALUES(6, 'EURO', 0);

----===================A. ALTER TABLES=========================
----==========================================================+
ALTER TABLE [dbo].[SuspiciousInfo]
ADD [CurrencyId]		INT NULL;
GO

UPDATE [dbo].[SuspiciousInfo] WITH(ROWLOCK, UPDLOCK)
SET [CurrencyId] = 0
WHERE [CurrencyId] IS NULL;

CREATE NONCLUSTERED INDEX NCIX_SuspiciousInfo_CurrencyId ON [dbo].[SuspiciousInfo] (
	[CurrencyId]		ASC
)
GO

ALTER TABLE [dbo].[ArbitrageTicketDetails]
ADD [CurrencyId]		INT NULL;
GO

UPDATE [dbo].[ArbitrageTicketDetails] WITH(ROWLOCK, UPDLOCK)
SET [CurrencyId] = 0
WHERE [CurrencyId] IS NULL;

CREATE NONCLUSTERED INDEX NCIX_ArbitrageTicketDetails_CurrencyId ON [dbo].[ArbitrageTicketDetails] (
	[CurrencyId]		ASC
)
GO

ALTER TABLE [dbo].[ChangedTicketGeneralInfo]
ADD [CurrencyId]		INT NULL;
GO

UPDATE cti WITH(ROWLOCK, UPDLOCK)
SET cti.[CurrencyId] = e.exchangeid
FROM [dbo].[ChangedTicketGeneralInfo] AS cti
	INNER JOIN bodb02.dbo.Exchange AS e ON cti.Currency = e.currency
WHERE [CurrencyId] IS NULL;

CREATE NONCLUSTERED INDEX NCIX_ChangedTicketGeneralInfo_CurrencyId ON [dbo].[ChangedTicketGeneralInfo] (
	[CurrencyId]		ASC
)
GO