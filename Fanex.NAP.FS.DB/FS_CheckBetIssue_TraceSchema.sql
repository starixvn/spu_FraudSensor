/*
	Created: 20190109@CaseyHuynh
	Task : CLEAR Table CheckBetScannedMatch. Only Keep data in 14 days
	DB: bodb_FraudSensor
	Original:

	Revisions:

	Param's Explanation (filtered by):
*/
-------------------------------------------------------------------------------
CREATE TABLE TempTrace_CheckBet (
	transid				BIGINT, 
	bettype				INT, 
	isCashOut			BIT, 
	isVoid				BIT, 
	custid				INT, 
	matchid				INT, 
	bettrans_stake		MONEY, 
	bets_stake			MONEY, 
	isOriginal			BIT, 
	isValidTP			TINYINT, 
	isValidOdds			TINYINT, 
	isValidBetteam		TINYINT,
	Step				INT,
	TraceTime			DATETIME
);
CREATE CLUSTERED INDEX IX_TempTrace_CheckBet_TransID_Bettype ON TempTrace_CheckBet(transid,bettype);

------------------------------------------------------------------------------------------------------
CREATE TABLE TempTrace_ChangedTickets(
	transid				BIGINT, 
	bettype				INT, 
	original_stake		MONEY,
	Step				INT,
	TraceTime			DATETIME
);
CREATE CLUSTERED INDEX IX_TempTrace_ChangedTickets_TransID_Bettype ON TempTrace_ChangedTickets(transid,bettype);
------------------------------------------------------------------------------------------------------


