SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_BSNG_PreCondition] 
AS
/*
	Created: 20180306@LONG
	Task : PreCondition for Big Stake Near The Goal fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20180312@CaseyHuynh: Correct Code Syntax
		- 20180525@Casey.Huynh: Exclude League Test

	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	
	
	DECLARE @CurrentFraudCaseId TINYINT;
	SET @CurrentFraudCaseId = 1; -- BSNG

	IF OBJECT_ID('tempdb..#tmpMatchCurrentScore') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatchCurrentScore;
	END
	
	CREATE TABLE #tmpMatchCurrentScore(
			MatchId			INT
		,	LiveHomeScore	SMALLINT
		,	LiveAwayScore	SMALLINT
	);

	CREATE CLUSTERED INDEX	#CIX_tmpMatchCurrentScoreIdx ON #tmpMatchCurrentScore (
			MatchId			ASC
	);

	IF OBJECT_ID('tempdb..#tmpNewMatch') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpNewMatch;
	END
	
	CREATE TABLE #tmpNewMatch(
			MatchId			INT PRIMARY KEY
	);

	IF OBJECT_ID('tempdb..#tmpMatchScoreChanged') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatchScoreChanged;
	END
	
	CREATE TABLE #tmpMatchScoreChanged(
			MatchId			INT PRIMARY KEY
	);

	-- Get current score of running matches
	INSERT INTO #tmpMatchCurrentScore(MatchId, LiveHomeScore, LiveAwayScore)
	SELECT [MatchId] = tch.matchid
		,	[LiveHomeScore] = MAX(tch.livehomescore)
		,	[LiveAwayScore] = MAX(tch.liveawayscore)
	FROM bodb02.dbo.match AS tch WITH (NOLOCK)
		INNER JOIN bodb02.dbo.league AS lea WITH (NOLOCK) ON tch.leagueid = lea.leagueid
	WHERE tch.eventstatus = 'running'
		AND tch.kickofftime <= GETDATE()
		AND tch.sporttype = 1
		AND lea.leaguename NOT LIKE '%TEST%'
	GROUP BY tch.matchid;

	-- Insert new matches into MatchLiveScore table
	INSERT INTO #tmpNewMatch(MatchId)
	SELECT r.MatchId
	FROM
		(
			SELECT MatchId FROM #tmpMatchCurrentScore WITH (NOLOCK)
			EXCEPT
			SELECT MatchId FROM dbo.MatchLiveScore WITH (NOLOCK)
		) AS r;

	INSERT INTO dbo.MatchLiveScore(MatchId, FraudCaseId, LiveHomeScore, LiveAwayScore)
	SELECT	[MatchId]			= t.MatchId
		,	[FraudCase]			= @CurrentFraudCaseId
		,	[LiveHomeScore]		= t.LiveHomeScore
		,	[LiveAwayScore]		= t.LiveAwayScore
	FROM #tmpMatchCurrentScore AS t WITH (NOLOCK)
	WHERE t.MatchId IN (SELECT MatchId FROM #tmpNewMatch WITH (NOLOCK));

	-- Get matches that have scores changed
	INSERT INTO #tmpMatchScoreChanged(MatchId)
	SELECT DISTINCT mls.MatchId
	FROM #tmpMatchCurrentScore AS t WITH (NOLOCK)
		INNER JOIN dbo.MatchLiveScore AS mls WITH (NOLOCK) ON mls.MatchId = t.MatchId
	WHERE mls.FraudCaseId = @CurrentFraudCaseId
		AND (mls.LiveHomeScore <> t.LiveHomeScore
		OR mls.LiveAwayScore <> t.LiveAwayScore);

	-- UPDATE live score back to MatchLiveScore table
	DELETE FROM dbo.MatchLiveScore WITH(ROWLOCK)
	WHERE FraudCaseId = 1 
		AND MatchId IN (SELECT MatchId FROM #tmpMatchScoreChanged WITH (NOLOCK));

	INSERT INTO dbo.MatchLiveScore(MatchId, FraudCaseId, LiveHomeScore, LiveAwayScore)
	SELECT	[MatchId]			= t.MatchId
		,	[FraudCase]			= @CurrentFraudCaseId
		,	[LiveHomeScore]		= t.LiveHomeScore
		,	[LiveAwayScore]		= t.LiveAwayScore
	FROM #tmpMatchCurrentScore AS t WITH (NOLOCK)
	WHERE t.MatchId IN (SELECT MatchId FROM #tmpMatchScoreChanged WITH (NOLOCK));

	SELECT MatchId
	FROM #tmpMatchScoreChanged WITH (NOLOCK);
END;
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_BSNG_PreCondition] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_BSNG_PreCondition] TO [bodbSPUNet]
GO 
