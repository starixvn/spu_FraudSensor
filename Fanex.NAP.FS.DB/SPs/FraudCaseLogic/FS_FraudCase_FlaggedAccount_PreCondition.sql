
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_FlaggedAccount_PreCondition] 
AS
/*
	Created: 20180308@LONG
	Task : PreCondition for Flagged Account fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20180312@CaseyHuynh: Correct Code Syntax
		- 20180525@Casey.Huynh: Exclude League Test
		
	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	

	IF OBJECT_ID('tempdb..#tmpMatches') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatches;
	END;

	CREATE TABLE #tmpMatches(
			MatchId			INT PRIMARY KEY
	);

	INSERT INTO #tmpMatches(MatchId)
	SELECT DISTINCT m.matchid AS MatchId
	FROM bodb02.dbo.match AS m WITH (NOLOCK)
		INNER JOIN bodb02.dbo.League AS l WITH(NOLOCK) ON m.leagueid = l.leagueid
	WHERE 
		m.eventstatus = 'running'
		AND m.sporttype = 1
		AND m.kickofftime <= GETDATE()
		AND l.isTest = 0
	UNION
	SELECT DISTINCT m.matchid AS MatchId
	FROM bodb02.dbo.match AS m WITH (NOLOCK)
		INNER JOIN bodb02.dbo.League AS l WITH(NOLOCK) ON m.leagueid = l.leagueid
		LEFT JOIN dbo.ScannedMatch AS ms WITH(NOLOCK)ON ms.matchid = m.matchid AND ms.FraudCaseId = 6
	WHERE ms.MatchId IS NULL
		AND m.eventstatus = 'completed'
		AND m.sporttype = 1
		AND m.kickofftime <= GETDATE()
		AND l.isTest = 0;

	SELECT DISTINCT MatchId
	FROM #tmpMatches WITH(NOLOCK);
END;

GO
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/
