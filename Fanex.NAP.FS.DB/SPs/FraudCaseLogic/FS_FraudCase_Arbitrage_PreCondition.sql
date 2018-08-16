USE [bodb_FraudSensor]
GO
SET QUOTED_IDENTIFIER ON ;
GO
SET ANSI_NULLS ON ;
GO
ALTER PROCEDURE [dbo].[FS_FraudCase_Arbitrage_PreCondition] 
AS
/*
	Created: 20180815@LONG
	Task : PreCondition for Arbitrage fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:

	Param's Explanation (filtered by):  
*/
BEGIN
	SET NOCOUNT ON;	

	SELECT	[MatchId] = m.matchid
	FROM	bodb02.dbo.Match AS m WITH(NOLOCK)
			INNER JOIN bodb02.dbo.league AS l WITH (NOLOCK) ON m.leagueid = l.leagueid
	WHERE	m.sporttype = 1
			AND m.eventstatus = 'Running'
			AND	m.MatchCode NOT LIKE '%hedge%'
			AND l.isTest = 0;
END;

GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_Arbitrage_PreCondition] TO [bodbSPUNet];
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_Arbitrage_PreCondition] TO [bodbSPUNet];
GO 
