/*
Creator: 20180406@LongLuu
Task:  Create tables for Fraud Sensor - Sprint#5
		+ 20180406@Casey: Alter Table DelayGoalAccount
Server:  bodb_FraudSensor
*/
ALTER TABLE [dbo].[DelayGoalAccount]
ADD [IsManualAdd]				BIT DEFAULT ((0)) NULL 
	,[IsActive]			BIT DEFAULT ((1)) NULL;
GO