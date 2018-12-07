GO
SET QUOTED_IDENTIFIER ON ;
GO
SET ANSI_NULLS ON ;
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_BetsNearGoal_PreCondition] 
AS
/*
	Created: 20180306@LONG
	Task : PreCondition for Bets Near The Goal fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20181207@Long: Created [Redmine ID: 106079]

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
	INSERT INTO dbo.MatchScore(MatchId, EventDate, EventStatus, TotalScores, HasScanned, UpdatedDate)
	SELECT	m.matchid
			, m.eventdate
			, m.eventstatus
			, m.livehomescore + liveawayscore
			, 0	
			, @CurrentTime	
	FROM	bodb02.dbo.Match AS m WITH(NOLOCK)
			INNER JOIN bodb02.dbo.league AS l WITH (NOLOCK) ON m.leagueid = l.leagueid
			LEFT JOIN dbo.MatchScore AS ms WITH(NOLOCK) ON m.matchid = ms.MatchId
	WHERE	ms.MatchId IS NULL
			AND (m.eventstatus = 'Running' OR m.eventstatus = 'Completed')			
			AND m.sporttype = 1
			AND l.isTest = 0
			AND m.kickofftime <= @CurrentTime	
			AND	m.livehomescore + liveawayscore > 0;
	
	--- UPDATE HASSCANNED = 1 FOR COMPLETED MATCH IF SCANNED ALL SCORED (EXCEPT THE LAST SCORE)
	UPDATE	ms 
	SET		ms.HasScanned = 1
			, ms.EventStatus = 'Completed'
	FROM	dbo.MatchScore AS ms WITH (ROWLOCK, UPDLOCK)
			INNER JOIN bodb02.dbo.Match m WITH(NOLOCK) ON m.matchid = ms.MatchId
	WHERE	ms.HasScanned = 0
			AND m.EventStatus = 'Completed'
			AND m.livehomescore + m.liveawayscore = ms.TotalScores;
	
	--- UPDATE NEW SCORE
	UPDATE	ms 
	SET		ms.TotalScores = m.livehomescore + m.liveawayscore
			, ms.UpdatedDate = @CurrentTime
	FROM	dbo.MatchScore AS ms WITH (ROWLOCK, UPDLOCK)
			INNER JOIN bodb02.dbo.Match AS m WITH(NOLOCK) ON m.MatchId = ms.MatchId
	WHERE	(ms.TotalScores) < (m.LiveHomeScore + m.LiveAwayScore)
			AND ms.HasScanned = 0;
	
	--- RETURN MATCH HAS NEW SCORE
	SELECT  ms.MatchId
	FROM	dbo.MatchScore AS ms WITH (NOLOCK)
	WHERE	ms.UpdatedDate = @CurrentTime
			AND ms.HasScanned = 0;			
END;

GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_BetsNearGoal_PreCondition] TO [bodbSPUNet];
GO 
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/
