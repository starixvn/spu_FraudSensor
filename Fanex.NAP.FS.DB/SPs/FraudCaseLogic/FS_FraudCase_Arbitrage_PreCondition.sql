USE [bodb_FraudSensor]
GO
/****** Object:  StoredProcedure [dbo].[FS_FraudCase_Arbitrage_PreCondition]    Script Date: 8/29/2018 9:39:47 AM ******/
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

	Param's Explanation (filtered by):  
*/
BEGIN
	SET NOCOUNT ON;	

	-- INIT
	DECLARE @CurrentTime DATETIME;
	
	SET @CurrentTime = GETDATE();

	IF OBJECT_ID('tempdb..#tmpMatches') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatches;
	END

	CREATE TABLE #tmpMatches
	(
		MatchId			INT PRIMARY KEY
	);

	IF OBJECT_ID('tempdb..#tmpMatchScoreHDP') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatchScoreHDP;
	END

	CREATE TABLE #tmpMatchScoreHDP
	(
			MatchId			INT
		,	BetType			INT
		,	Handicap		MONEY
		,	TotalScores		INT
		,	MaxTransId		BIGINT
	);

	CREATE CLUSTERED INDEX	#CIX_tmpMatchScoreHDPIdx ON #tmpMatchScoreHDP (
			MatchId			ASC
		,	BetType			ASC
		,	Handicap		ASC
		,	TotalScores		ASC
	);

	-- START
	INSERT INTO #tmpMatches(MatchId)
	SELECT	[MatchId] = m.matchid
	FROM	bodb02.dbo.Match AS m WITH(NOLOCK)
			INNER JOIN bodb02.dbo.league AS l WITH (NOLOCK) ON m.leagueid = l.leagueid
	WHERE	m.sporttype = 1
			AND m.eventstatus = 'Running'
			AND	m.MatchCode NOT LIKE '%hedge%'
			AND m.kickofftime <= @CurrentTime
			AND l.isTest = 0;
				
	INSERT INTO #tmpMatchScoreHDP(MatchId, BetType, Handicap, TotalScores, MaxTransId)
	SELECT	DISTINCT b.MatchId
		,	b.bettype
		,	(b.hdp1 - b.hdp2)
		,	(b.livehomescore + b.liveawayscore)
		,	MAX(b.transid)
	FROM bodb02.dbo.bettrans AS b WITH (NOLOCK)
		INNER JOIN #tmpMatches AS m WITH(NOLOCK) ON b.matchid = m.MatchId
	WHERE b.bettype IN (1, 3, 7, 8)
		AND b.Currency NOT IN (20, 27, 28)
	GROUP BY b.MatchId, b.bettype, (b.hdp1 - b.hdp2), (b.livehomescore + b.liveawayscore);

	INSERT INTO [dbo].[ArbitrageMatchScoreHDP](MatchId, BetType, Handicap, TotalScores, MaxTransId, HasScanned, UpdatedDate)
	SELECT	b.MatchId
		,	b.BetType
		,	b.Handicap
		,	b.TotalScores
		,	b.MaxTransId
		,	0
		,	GETDATE()
	FROM #tmpMatchScoreHDP AS b WITH(NOLOCK)
		LEFT JOIN [dbo].[ArbitrageMatchScoreHDP] AS ms WITH(NOLOCK) ON b.matchid = ms.MatchId 
			AND b.Handicap = ms.Handicap AND b.TotalScores = ms.TotalScores
	WHERE ms.MatchId IS NULL
	GROUP BY b.matchid, b.BetType, b.Handicap, b.TotalScores, b.MaxTransId;

	-- RETURN DATA
	SELECT DISTINCT [MatchId] = ms.MatchId
		,	[CustId] = b.custid
		,	[BetType] = b.bettype
		,	[Handicap] = (b.hdp1 - b.hdp2)
		,	[LiveHomeScore] = b.livehomescore
		,	[LiveAwayScore] = b.liveawayscore
	FROM #tmpMatches AS ms WITH(NOLOCK)
		INNER JOIN bodb02.dbo.bettrans AS b WITH (NOLOCK) ON ms.MatchId = b.matchid
		INNER JOIN (
						SELECT ms.MatchId, MIN(ms.MaxTransId) AS TransId
						FROM [dbo].[ArbitrageMatchScoreHDP] AS ms WITH(NOLOCK)
						WHERE ms.HasScanned = 0
						GROUP BY ms.MatchId
					) AS t ON b.matchid = t.MatchId
	WHERE b.transid >= t.TransId
		AND b.bettype IN (1, 3, 7, 8)
		AND b.Currency NOT IN (20, 27, 28);

	--- UPDATE HASSCANNED = 1 FOR THE LAST UNSCANNED RECORDS
	UPDATE	ms 
	SET		ms.HasScanned = 1
	FROM	[dbo].[ArbitrageMatchScoreHDP] AS ms WITH (ROWLOCK, UPDLOCK)
	WHERE	ms.HasScanned = 0
		AND ms.MaxTransId < (	SELECT MAX(m.MaxTransId)
								FROM [dbo].[ArbitrageMatchScoreHDP] AS m WITH(NOLOCK)
								WHERE m.HasScanned = 0 AND m.MatchId = ms.MatchId
								GROUP BY m.MatchId
								HAVING COUNT(1) > 1);
	
	--- UPDATE MAXTRANSID FOR EXISTED ONES
	UPDATE	ms 
	SET		ms.MaxTransId = m.MaxTransId
		,	ms.UpdatedDate = @CurrentTime
	FROM	[dbo].[ArbitrageMatchScoreHDP] AS ms WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpMatchScoreHDP AS m WITH(NOLOCK) ON m.MatchId = ms.MatchId
			AND m.Handicap = ms.Handicap AND m.TotalScores = ms.TotalScores
	WHERE	ms.HasScanned = 0;	
END;

GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_Arbitrage_PreCondition] TO [bodbSPUNet];
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_Arbitrage_PreCondition] TO [bodbSPUNet];
GO 
