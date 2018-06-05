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

CREATE NONCLUSTERED INDEX NIX_OfficeIP_IPRange ON dbo.OfficeIP (
	IPRange		ASC
) ON IDX;

CREATE NONCLUSTERED INDEX NIX_OfficeIP_FromNumberToNumber ON dbo.OfficeIP (
	FromNumber		ASC
	,ToNumber		ASC
) ON IDX;
GO

GO

----===================C. ALTER TABLES=========================
----===========================================================
