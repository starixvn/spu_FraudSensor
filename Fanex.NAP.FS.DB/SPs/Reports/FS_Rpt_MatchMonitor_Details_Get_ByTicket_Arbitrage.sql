
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER PROCEDURE [dbo].[FS_Rpt_MatchMonitor_Details_Get_ByTicket_Arbitrage]
		@MatchId			INT
	,	@EventDate			DATETIME
	,	@FraudCaseId		INT	
AS
/*
	Created: 20180815@Long
	Task : Get Tickets by Match & Fraud Case for Arbitrage
	DB: bodb_FraudSensor
	Original:

	Revisions:

	Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	--===	1.1 INITIAL TEMP TABLES #tmpBUMB: Include Balance Up and Move Bet Days	
	IF OBJECT_ID('tempdb..#tmpBUMB') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpBUMB;
	END;
	
	CREATE TABLE #tmpBUMB (
			BUDay		SMALLDATETIME
		,	MBDay		SMALLDATETIME
	 );
	 
	 --===	1.2 INITIAL: TEMP TABLES #tmpFraudTickets: GET Fraud Tickets
	IF OBJECT_ID('tempdb..#tmpTransId') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpFraudTickets;
	END;
	
	CREATE TABLE #tmpFraudTickets (
			TransID	BIGINT
	 );

	--===	2.1 GET: Balance Up Day and Move Bet Day
	INSERT INTO #tmpBUMB(BUDay,MBDay)
	EXECUTE bodb02.dbo.Age_SelBalanceUpDay;

	DECLARE @IsAfterMoveBetDay BIT = 1;

	SELECT TOP(1) @IsAfterMoveBetDay = 0
	FROM #tmpBUMB WITH(NOLOCK)
	WHERE @EventDate BETWEEN BUDay AND MBDay;
	
	--===	2.2 GET: Fraud Tickets Insert to #tmpFraudTickets	
	--INSERT INTO #tmpFraudTickets(TransId)
	INSERT INTO #tmpFraudTickets(TransID)
	SELECT VAL
	FROM dbo.SuspiciousInfo si WITH(NOLOCK)
		 CROSS APPLY dbo.SPU_StrSplit(si.transids, ',')
	WHERE	si.MatchId = @MatchId
			AND si.FraudCaseId = @FraudCaseId;
			
	--===	2.2 GET: Homeid and AwayId
	DECLARE @homeId INT;
	DECLARE @awayId INT;
			
	--===	3.1 RETURN: Fraud Tickets Info				
	If @IsAfterMoveBetDay = 1
		BEGIN
			SELECT @homeId = m.homeid
					, @awayId = m.awayid
			FROM bodb02.dbo.match m WITH(NOLOCK)
			WHERE m.matchid = @MatchId;
			
			SELECT	[Username]		= b.Username
				,	[DangerLevel]	= c.danger2 
				,	[BettypeId]		= b.bettype 
				,	[TransId]		= b.Transid
				,	[TransDate]		= b.transdate
				,	[BetId]			= b.betid
				,	[BetTeam]		= b.betteam
				,	[Handicap1]		= b.hdp1	
				,	[Handicap2]		= b.hdp2	
				,	[LiveHomeScore] = ISNULL(b.livehomescore,0) 
				,	[LiveAwayScore] = ISNULL(b.liveawayscore,0) 
				,	[IsLive]		= b.liveindicator 
				,	[Odds]			= b.odds
				,	[OddsType]		= b.oddstype
				,	[CurrencyName]	= b.currencyname
				,	[FullStake]		= b.stake * b.actualrate 
				,	[ComPTStake]	= b.stake * (1 - b.tpositiontaking) * b.actualrate 
				,	[VirtualStake]	= b.stake * (1 - b.tpositiontaking) * b.virtualrate 
			FROM bodb02.dbo.bettrans b WITH(NOLOCK)
				INNER JOIN #tmpFraudTickets f WITH(NOLOCK) ON b.transid = f.TransID
				INNER JOIN bodb02.dbo.custInfo c WITH(NOLOCK) ON b.custid = c.custid;
		END;
	ELSE
		BEGIN		
			SELECT @homeId = m.homeid
					, @awayId = m.awayid
			FROM bodb02.dbo.match14 m WITH(NOLOCK)
			WHERE m.matchid = @MatchId;
			
			SELECT	[Username]		= b.Username
				,	[DangerLevel]	= c.danger2 
				,	[BettypeId]		= b.bettype 
				,	[TransId]		= b.Transid
				,	[TransDate]		= b.transdate
				,	[BetId]			= b.betid
				,	[BetTeam]		= b.betteam
				,	[Handicap1]		= b.hdp1	
				,	[Handicap2]		= b.hdp2	
				,	[LiveHomeScore] = ISNULL(b.livehomescore,0) 
				,	[LiveAwayScore] = ISNULL(b.liveawayscore,0) 
				,	[IsLive]		= b.liveindicator 
				,	[Odds]			= b.odds
				,	[OddsType]		= b.oddstype
				,	[CurrencyName]	= b.currencyname
				,	[FullStake]		= b.stake * b.actualrate 
				,	[ComPTStake]	= b.stake * (1 - b.tpositiontaking) * b.actualrate 
				,	[VirtualStake]	= b.stake * (1 - b.tpositiontaking) * b.virtualrate 
			FROM bodb02.dbo.bettrans14 b WITH(NOLOCK)
				INNER JOIN #tmpFraudTickets f WITH(NOLOCK) ON b.transid = f.TransID
				INNER JOIN bodb02.dbo.custInfo c WITH(NOLOCK) ON b.custid = c.custid;
		END;
END;

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 GRANT EXECUTE ON [dbo].[FS_Rpt_MatchMonitor_Details_Get_ByTicket_Arbitrage] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_Rpt_MatchMonitor_Details_Get_ByTicket_Arbitrage] TO [bodbSPUNet]
GO 
