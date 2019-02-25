ALTER PROCEDURE [dbo].[FS_Rpt_MatchMonitor_Details_Get_ByTicket_Arbitrage]
		@MatchId					INT
	,	@EventDate					DATETIME
	,	@IsShowSpecialCurrencies	BIT = 0
AS
/*
	Created: 20180815@Long
	Task : Get Tickets by Match & Fraud Case for Arbitrage
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20180815@Long: Implement Get Tickets by Match & Fraud Case for Arbitrage
		- 20181114@Long: Adding View Permission on Special Currencies [Redmine ID: #104707]
		- 20190219@John: Update logic Special Currencies and seperate Arbitrage [Redmine ID: #99753]

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
	IF OBJECT_ID('tempdb..#tmpFraudTickets') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpFraudTickets;
	END;
	
	CREATE TABLE #tmpFraudTickets (
			GroupId		INT
		,	TransId		BIGINT
		,	IsIPFraud	BIT
	 );

	CREATE CLUSTERED INDEX #CIX_tmpFraudTickets_TransID ON #tmpFraudTickets (
			TransID			ASC
	);

	--===	2.1 GET: Balance Up Day and Move Bet Day
	INSERT INTO #tmpBUMB(BUDay,MBDay)
	EXECUTE bodb02.dbo.Age_SelBalanceUpDay;

	DECLARE @IsAfterMoveBetDay BIT = 1;

	SELECT TOP(1) @IsAfterMoveBetDay = 0
	FROM #tmpBUMB WITH(NOLOCK)
	WHERE @EventDate BETWEEN BUDay AND MBDay;
	
	--===	2.2 GET: Fraud Tickets Insert to #tmpFraudTickets	
	INSERT INTO #tmpFraudTickets(GroupId, TransId, IsIPFraud)
	SELECT si.GroupId, VAL, si.IsIPFraud
	FROM dbo.ArbitrageTicketDetails si WITH(NOLOCK)
		INNER JOIN dbo.SuspiciousCustomer c WITH(NOLOCK) ON si.CustId = c.custid
		CROSS APPLY dbo.SPU_StrSplit(si.transids, ',')
	WHERE	si.MatchId = @MatchId
		AND (@IsShowSpecialCurrencies = 1 OR c.CurrencyId NOT IN (SELECT CurrencyId FROM dbo.SpecialCurrency WITH(NOLOCK)));
			
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
			
			SELECT	[custid]		= b.custid
				,	[Username]		= b.Username
				,	[DangerLevel]	= c.danger2 
				,	[BettypeId]		= b.bettype 
				,	[TransId]		= b.Transid
				,	[GroupId]		= f.GroupId
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
				,	[IsIPFraud]		= f.IsIPFraud
			FROM bodb02.dbo.bettrans b WITH(NOLOCK)
				INNER JOIN #tmpFraudTickets f WITH(NOLOCK) ON b.transid = f.TransID
				INNER JOIN bodb02.dbo.custInfo c WITH(NOLOCK) ON b.custid = c.custid
			WHERE b.winlostdate = @EventDate;

		END;
	ELSE
		BEGIN		
			SELECT @homeId = m.homeid
					, @awayId = m.awayid
			FROM bodb02.dbo.match14 m WITH(NOLOCK)
			WHERE m.matchid = @MatchId;
			
			SELECT	[custid]		= b.custid
				,	[Username]		= b.Username
				,	[DangerLevel]	= c.danger2 
				,	[BettypeId]		= b.bettype 
				,	[TransId]		= b.Transid
				,	[GroupId]		= f.GroupId
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
				,	[IsIPFraud]		= f.IsIPFraud
			FROM bodb02.dbo.bettrans14 b WITH(NOLOCK)
				INNER JOIN #tmpFraudTickets f WITH(NOLOCK) ON b.transid = f.TransID
				INNER JOIN bodb02.dbo.custInfo c WITH(NOLOCK) ON b.custid = c.custid
			WHERE b.winlostdate = @EventDate;

		END;
END;

