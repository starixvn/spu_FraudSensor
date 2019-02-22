USE bodb_FraudSensor
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
ALTER PROCEDURE dbo.FS_FraudCase_OfficeIP_PreCondition

AS
/*
	Created: 20180605@CaseyHuynh
	Task : Task IP Office Precondition
	DB: bodb_FraudSensor

	Original:

	Revisions: 

	Param's Explanation (filtered by):

*/
BEGIN
	SET NOCOUNT ON;
	
	-- Arrange tables and initialize
	IF OBJECT_ID('tempdb..#tmpSporttype') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpSporttype;
	END
	
	CREATE TABLE #tmpSporttype(
		Sporttype	INT
	);
	
	--- GET SPORTBOOK SPORTTYPE
	INSERT INTO #tmpSporttype (Sporttype)
	SELECT	DISTINCT(Sporttype)
	FROM	bodb02.dbo.SportBettypeMatrix sbm WITH (NOLOCK)
	WHERE	sbm.ProductID = 1;
	
	SELECT	m.matchid, m.eventdate
	FROM	bodb02.dbo.match m WITH (NOLOCK)
			INNER JOIN #tmpSporttype s WITH (NOLOCK) ON s.sporttype = m.sporttype
			INNER JOIN bodb02.dbo.league l WITH (NOLOCK) ON m.leagueid = l.leagueid
			LEFT JOIN dbo.ScannedMatch sm WITH (NOLOCK) ON m.matchid = sm.matchid AND sm.FraudCaseId = 2	
	WHERE	m.eventstatus = 'completed'
			AND l.isTest = 0
			AND sm.MatchId IS NULL;

END;
GO 
 GRANT EXECUTE ON dbo.FS_FraudCase_OfficeIP_PreCondition TO bodbSPUNet;
GO 
 GRANT VIEW DEFINITION ON dbo.FS_FraudCase_OfficeIP_PreCondition TO bodbSPUNet;