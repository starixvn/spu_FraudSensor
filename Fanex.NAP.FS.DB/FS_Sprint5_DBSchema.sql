/*
Creator: 20180406@LongLuu
Task:  Create tables for Fraud Sensor - Sprint#5
		+ 20180406@Casey: 
			> Alter Table DelayGoalAccount
			> Drop tables: EvidenceTypes, FlaggedAccount
Server:  bodb_FraudSensor
*/ 
--===========ALTER TABLES
ALTER TABLE [dbo].[DelayGoalAccount]
ADD		[IsManualAdd]		BIT DEFAULT ((0)) NULL 
	,	[IsActive]			BIT DEFAULT ((1)) NULL;

GO
ALTER TABLE [dbo].[DelayGoalAccount]
ADD CONSTRAINT PK_DelayGoalAccount PRIMARY KEY
(
		CustId				ASC
	,	CreatedBy			ASC
)

GO

/* Update primary key of DelayGoalAccount */
DECLARE @table NVARCHAR(512), @sql NVARCHAR(MAX);
SELECT @table = N'dbo.[DelayGoalAccount]';

SELECT @sql = 'ALTER TABLE ' + @table + ' DROP CONSTRAINT ' + name + ';'
FROM sys.key_constraints
WHERE [type] = 'PK'
	AND [parent_object_id] = OBJECT_ID(@table);

EXEC sp_executeSQL @sql;

GO --=========DROP TABLES
IF OBJECT_ID('tempdb..#FlaggedAccount') IS NOT NULL
	BEGIN
		DROP TABLE	#FlaggedAccount;
	END

IF OBJECT_ID('tempdb..#EvidenceTypes') IS NOT NULL
	BEGIN
		DROP TABLE	#EvidenceTypes;
	
	END	
	
