USE [bodb_FraudSensor]
GO
SET QUOTED_IDENTIFIER ON ;
GO
SET ANSI_NULLS ON ;
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
		- 20180525@Casey.Huynh: Update set LiveScore = 0 for New Match when insert to MatchLiveScore
		- 20180605@Casey.Huynh: Enhance SP
	Param's Explanation (filtered by):  
*/
BEGIN
	SET NOCOUNT ON;	
	
	DECLARE @CurrentTime DATETIME;
	
	SET @CurrentTime = GETDATE();
	
	IF OBJECT_ID('tempdb..#tmpChangedMatch') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpChangedMatch;
	END
	
	CREATE TABLE #tmpChangedMatch(
		MatchID INT
		, EventDate	DATETIME
		, EventStatus	NVARCHAR(100)
		, TotalScores	TINYINT
		, HasScanned	BIT
	);
	
	--- ============1. INSERT NEW MATCH=============================
	INSERT INTO dbo.MatchScore(MatchId, EventDate, EventStatus, TotalScores, HasScanned)
	SELECT	m.matchid
			, m.eventdate
			, m.eventstatus
			, 0
			, 0		
	FROM	bodb02.dbo.Match AS m WITH(NOLOCK)
			INNER JOIN bodb02.dbo.league AS l WITH (NOLOCK) ON m.leagueid = l.leagueid
			LEFT JOIN dbo.MatchScore AS ms WITH(NOLOCK) ON m.matchid = ms.MatchId
	WHERE	ms.MatchId IS NULL
			AND m.sporttype = 1
			AND l.isTest = 0
			AND m.kickofftime <= @CurrentTime;	

	
	--- UPDATE NEW SCORE
	UPDATE	ms WITH (ROWLOCK, UPDLOCK)
	SET		ms.TotalScores = m.livehomescore + m.liveawayscore
			, ms.UpdatedDate = @CurrentTime
	FROM	dbo.MatchScore AS ms
			INNER JOIN bodb02.dbo.Match AS m WITH(NOLOCK) ON m.MatchId = ms.MatchId
	WHERE	(ms.TotalScores) < (m.LiveHomeScore + m.LiveAwayScore)
			AND ms.HasScanned = 0;	 
			
	--- UPDATE HASSCANNED = 1 FOR COMPLETED MATCH IF SCANNED ALL SCORED (EXCEPT THE LAST SCORE)
	UPDATE	ms 
	SET		ms.HasScanned = 1
	FROM	dbo.MatchScore AS ms WITH (ROWLOCK, UPDLOCK)
			INNER JOIN bodb02.dbo.Match AS m WITH(NOLOCK) ON m.MatchId = ms.MatchId
	WHERE	(ms.TotalScores) = (m.LiveHomeScore + m.LiveAwayScore)
			AND ms.HasScanned = 0
			AND ms.EventStatus = 'Completed'
			AND (ms.TotalScores) - 1 = (SELECT MAX(si.LiveHomeScore + si.LiveAwayScore)
														FROM dbo.BSNGScanInfo AS si WITH(NOLOCK)
														WHERE si.MatchId = ms.MatchId
																AND si.EventDate = ms.EventDate
																AND si.HasScanned = 1 );	
	--- RETURN MATCH HAS NEW SCORE
	SELECT  DISTINCT ms.MatchId
	FROM	dbo.MatchScore AS ms WITH (NOLOCK)
	WHERE	ms.UpdatedDate = @CurrentTime
			AND ms.HasScanned = 0;
	
END;
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_BSNG_PreCondition] TO [bodbSPUNet];
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_BSNG_PreCondition] TO [bodbSPUNet];
GO 
