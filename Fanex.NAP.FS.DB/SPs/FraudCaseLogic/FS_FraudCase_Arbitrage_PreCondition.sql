USE [bodb_FraudSensor]
GO
/****** Object:  StoredProcedure [dbo].[FS_FraudCase_Arbitrage_PreCondition_20180904]    Script Date: 09/04/2018 19:56:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[FS_FraudCase_Arbitrage_PreCondition]
AS
/*
	Created: 20180815@LONG
	Task : PreCondition for Arbitrage fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions: 
		- 20180815@LONG: Implement PreCondition for Arbitrage fraud case
		- 20180926@LONG: Remove redundant fraud tickets

	Param's Explanation (filtered by):  
	
*/
BEGIN
	SET NOCOUNT ON;	

	-- INIT
	DECLARE @CurrentTime DATETIME;
	
	SET @CurrentTime = GETDATE();
	
	---================SCANNING MATCH=========================
		IF OBJECT_ID('tempdb..#tmpScanMatches') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpScanMatches;
	END

	CREATE TABLE #tmpScanMatches
	(
		MatchId			INT PRIMARY KEY
		, IsMatchCompleted	BIT
	);
	--======================================

	IF OBJECT_ID('tempdb..#tmpMatchRunning') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatchRunning;
	END

	CREATE TABLE #tmpMatchRunning
	(
		MatchId			INT PRIMARY KEY
	);

	---======================================================
		IF OBJECT_ID('tempdb..#tmpMatchCompleted') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatchCompleted;
	END

	CREATE TABLE #tmpMatchCompleted
	(
		MatchId			INT PRIMARY KEY
	);
	
	---=======================================================
	IF OBJECT_ID('tempdb..#tmpMatchMaxTransId') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatchMaxTransId;
	END

	CREATE TABLE #tmpMatchMaxTransId
	(
			MatchId				INT
		,	MaxTransId			BIGINT
		,   MaxLiveHomeScore	INT
		,	MaxLiveAwayScore	INT
		,	IsMatchCompleted	INT
	);
	---===========================================================
		IF OBJECT_ID('tempdb..#tmpCustomerList') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpCustomerList;
	END

	CREATE TABLE #tmpCustomerList
	(
			MatchId				INT
		,	Bettype				INT
		,	Handicap			SMALLMONEY
		,   LiveHomeScore		INT
		,	LiveAwayScore		INT
		,	CustId				INT
	);
	--============================================================
	--- GET Match Completed that has not Completed Scanning
	INSERT INTO #tmpScanMatches(MatchId, IsMatchCompleted)
	SELECT	m.matchid
			, 1 AS IsMatchCompleted
	FROM	bodb02.dbo.Match AS m WITH(NOLOCK)
			INNER JOIN dbo.ArbitrageMatchScanning AS ams WITH (NOLOCK) ON m.matchid = ams.MatchId
	WHERE ams.IsScanCompleted = 0
			AND m.eventstatus = 'Completed';
	
	-- GET Match Running
	INSERT INTO #tmpScanMatches(MatchId, IsMatchCompleted)
	SELECT	m.matchid
			, 0 AS IsMatchCompleted
	FROM	bodb02.dbo.Match AS m WITH(NOLOCK)
			INNER JOIN bodb02.dbo.league AS l WITH (NOLOCK) ON m.leagueid = l.leagueid
	WHERE	m.sporttype			= 1
			AND m.eventstatus	= 'Running'
			AND	m.MatchCode		NOT LIKE '%hedge%'
			AND m.kickofftime <= @CurrentTime	
			AND l.isTest = 0;
	
		--- GET Current MaxTransId
	INSERT INTO #tmpMatchMaxTransId(MatchId, IsMatchCompleted, MaxTransId, MaxLiveHomeScore, MaxLiveAwayScore)	
	SELECT	m.MatchID
			, m.IsMatchCompleted
			, MAX(b.TransId)		AS MaxTransId
			, MAX(b.Livehomescore)	AS MaxLiveHomeScore
			, MAX(b.LiveawayScore)	AS MaxLiveAwayScore
	FROM	#tmpScanMatches AS m WITH(NOLOCK)
			INNER JOIN bodb02.dbo.bettrans AS b WITH(NOLOCK)ON m.MatchId = b.matchid
	GROUP BY m.matchid, m.IsMatchCompleted;	
	
	---Insert New Match to ArbitrageGoalMatchScanning: LastScannedTransId default = 0 
	INSERT INTO dbo.ArbitrageMatchScanning (Matchid, MaxTransId, LiveHomeScore, LiveAwayScore, CapturedDate, IsScanCompleted, IsMatchCompleted)	
	SELECT	mr.matchid	AS MatchId
		,	0 AS MaxTransid
		,	mr.MaxLiveHomeScore 
		,	mr.MaxLiveAwayScore
		,	CapturedDate	= GETDATE()
		,	IsScanCompleted	= 0
		,	mr.IsMatchCompleted
	FROM #tmpMatchMaxTransId AS mr WITH(NOLOCK)
		LEFT JOIN dbo.ArbitrageMatchScanning AS ms WITH(NOLOCK)ON mr.matchid = ms.matchid
	WHERE ms.Matchid IS NULL;	
	
	--- GET CUSTID Has not been Scanned
	INSERT INTO #tmpCustomerList(MatchId, Bettype, Handicap, LiveHomeScore, LiveAwayScore, CustId)
	SELECT  DISTINCT MatchId = b.MatchId		
		,	BetType = b.bettype
		,	Handicap = (b.hdp1 - b.hdp2)
		,	LiveHomeScore = b.livehomescore
		,	LiveAwayScore = b.liveawayscore
		,	CustId = b.custid
	FROM	bodb02.dbo.bettrans AS b WITH (NOLOCK) 
			INNER JOIN dbo.ArbitrageMatchScanning AS a WITH (NOLOCK) ON b.matchid = a.matchid
	WHERE	b.transid > a.MaxTransId
			AND b.bettype IN (1, 3, 7, 8)
			AND b.Currency NOT IN (20, 27, 28)
			AND a.IsScanCompleted = 0
	GROUP BY b.matchid, b.BetType, b.hdp1,b.hdp2, b.livehomescore,b.liveawayscore, b.custid;	
	
	----- RETURN CUSTOMER LIST
	SELECT MatchId
			, Bettype
			, Handicap
			, LiveHomeScore
			, LiveAwayScore
			, CustId
	FROM	#tmpCustomerList WITH (NOLOCK)
	ORDER BY MatchId, Bettype, Handicap, LiveHomeScore, LiveAwayScore, CustId;
	
	
	---- NEW ArbitrageMatchScoreHDP
	INSERT INTO ArbitrageMatchScoreHDP (MatchId, BetType, Handicap, TotalScores, HasScanned, UpdatedDate)
	SELECT		cl.MatchId
				, cl.BetType
				, cl.Handicap
				, (cl.LiveAwayScore+cl.LiveHomeScore) AS TotalScores
				, 0 AS HasScanned
				, @CurrentTime AS UpdatedDate
	FROM		#tmpCustomerList AS cl WITH (NOLOCK)
				LEFT JOIN ArbitrageMatchScoreHDP AS ams WITH (NOLOCK) 
					ON cl.MatchId = ams.MatchId AND cl.Bettype = ams.BetType AND cl.Handicap = ams.Handicap AND (cl.LiveAwayScore+cl.LiveHomeScore) = ams.TotalScores
	WHERE		ams.MatchId IS NULL
	GROUP BY	cl.MatchId, cl.Bettype, cl.Handicap, cl.LiveHomeScore, cl.LiveAwayScore;
	
	------UPDATE ArbitrageMatchScoreHDP
	UPDATE	ams
	SET		ams.HasScanned = 1
	FROM	dbo.ArbitrageMatchScoreHDP AS ams WITH (NOLOCK)
					INNER JOIN #tmpCustomerList AS cl WITH (NOLOCK)
						ON cl.MatchId = ams.MatchId AND (cl.LiveAwayScore+cl.LiveHomeScore) > ams.TotalScores
			WHERE	ams.HasScanned = 0;
	
	UPDATE	ams
	SET		ams.HasScanned = 1
	FROM	dbo.ArbitrageMatchScoreHDP AS ams WITH (NOLOCK)
					INNER JOIN #tmpMatchMaxTransId AS cl WITH (NOLOCK) ON cl.MatchId = ams.MatchId
			WHERE	ams.HasScanned = 0
					AND cl.IsMatchCompleted = 1;
	
	------UPDATE MaxTransId for Matches
	UPDATE	am
	SET		am.maxTransId = mm.MaxTransId
			, am.LiveHomeScore = mm.MaxLiveHomeScore
			, am.LiveAwayScore = mm.MaxLiveAwayScore
			, am.IsScanCompleted = mm.IsMatchCompleted
			, am.IsMatchCompleted = mm.IsMatchCompleted
	FROM	dbo.ArbitrageMatchScanning AS am WITH (ROWLOCK)
				INNER JOIN #tmpMatchMaxTransId  AS mm WITH (NOLOCK) ON am.matchid = mm.matchid;		

END;

GO
GRANT EXECUTE ON [dbo].[FS_FraudCase_Arbitrage_PreCondition] TO [bodbSPUNet]
GO
GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_Arbitrage_PreCondition] TO [bodbSPUNet]
GO

GO
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/

