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
		- 
	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	
	
	DECLARE @CurrentTime DATETIME;
	
	SET @CurrentTime = GETDATE();
		
	--- INSERT NEW MATCH
	INSERT INTO MatchScore(MatchId, EventDate, TotalScores, HasScanned)
	SELECT	m.matchid
			, m.eventdate
			, 0
			, 0		
	FROM	bodb02.dbo.Match m WITH(NOLOCK)
			INNER JOIN bodb02.dbo.league AS l WITH (NOLOCK) ON m.leagueid = l.leagueid
			LEFT JOIN MatchScore ms WITH(NOLOCK) ON m.matchid = ms.MatchId
	WHERE	ms.MatchId IS NULL
			AND m.sporttype = 1
			AND l.isTest = 0
			AND m.kickofftime <= @CurrentTime
	
	--- UPDATE NEW SCORE
	UPDATE	ms
	SET		ms.TotalScores = m.livehomescore + m.liveawayscore
			, ms.UpdatedDate = @CurrentTime
	FROM	MatchScore ms WITH(ROWLOCK)
			INNER JOIN bodb02.dbo.Match m WITH(NOLOCK) ON m.MatchId = ms.MatchId
	WHERE	(ms.TotalScores) > (m.LiveHomeScore + m.LiveAwayScore)
			AND ms.HasScanned = 0;
	
	--- RETURN MATCH HAS NEW SCORE
	SELECT  DISTINCT ms.MatchId
	FROM	dbo.MatchScore ms WITH (NOLOCK)
	WHERE	ms.UpdatedDate = @CurrentTime;
	
END;
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_BSNG_PreCondition] TO [bodbSPUNet];
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_BSNG_PreCondition] TO [bodbSPUNet];
GO 
