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

	SELECT	m.matchid ,m.eventdate
	FROM	bodb02.dbo.match m WITH (NOLOCK)
			INNER JOIN bodb02.dbo.league l WITH (NOLOCK) ON m.leagueid = l.leagueid
			LEFT JOIN dbo.ScannedMatch sm WITH (NOLOCK) ON m.matchid = sm.matchid AND sm.FraudCaseId = 2	
	WHERE	m.sporttype BETWEEN 1 AND 99
			AND m.eventstatus = 'completed'
			AND l.isTest = 0
			AND sm.MatchId IS NULL;
END;
GO 
 GRANT EXECUTE ON dbo.FS_FraudCase_OfficeIP_PreCondition TO bodbSPUNet;
GO 
 GRANT VIEW DEFINITION ON dbo.FS_FraudCase_OfficeIP_PreCondition TO bodbSPUNet;
