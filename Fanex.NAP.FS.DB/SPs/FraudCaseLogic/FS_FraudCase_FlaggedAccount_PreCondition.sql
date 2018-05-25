
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

	SELECT DISTINCT m.matchid AS MatchId
	FROM bodb02.dbo.match AS m WITH (NOLOCK)
		INNER JOIN bodb02.dbo.League AS l WITH(NOLOCK) ON m.leagueid = l.leagueid
	WHERE 
		m.eventstatus = 'running'
		AND m.sporttype = 1
		AND m.kickofftime <= GETDATE()
		AND l.isTest = 0;
END;
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_FlaggedAccount_PreCondition] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_FlaggedAccount_PreCondition] TO [bodbSPUNet]
GO 
