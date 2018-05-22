
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

	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	

	SELECT DISTINCT tch.matchid AS MatchId
	FROM bodb02.dbo.match AS tch WITH (NOLOCK)
		INNER JOIN bodb02.dbo.sports AS rts WITH (NOLOCK) ON tch.sporttype = rts.sporttype
	WHERE 
		tch.eventstatus = 'running'
		AND rts.sporttype = 1
		AND tch.kickofftime <= GETDATE();
END;
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_FlaggedAccount_PreCondition] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_FlaggedAccount_PreCondition] TO [bodbSPUNet]
GO 
