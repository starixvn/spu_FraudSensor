
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_FlaggedAccount_PreCondition_ForCompletedMatches] 
AS
/*
	Created: 20181207@LONG
	Task : PreCondition for Flagged Account fraud case FOR COMPLETED MATCHES
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20181208@Long: Scan for completed matches [Redmine ID: #106073]
		
	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	

	SELECT DISTINCT m.matchid AS MatchId
	FROM bodb02.dbo.match AS m WITH (NOLOCK)
		INNER JOIN bodb02.dbo.League AS l WITH(NOLOCK) ON m.leagueid = l.leagueid
	WHERE 
		m.eventstatus = 'completed'
		AND m.sporttype = 1
		AND l.isTest = 0
		AND m.eventdate = '2018-12-06 00:00:00.000';
END;
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_FlaggedAccount_PreCondition_ForCompletedMatches] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_FlaggedAccount_PreCondition_ForCompletedMatches] TO [bodbSPUNet]
