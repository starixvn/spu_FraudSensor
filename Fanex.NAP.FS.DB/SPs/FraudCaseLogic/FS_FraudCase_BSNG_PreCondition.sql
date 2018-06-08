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
	
	INSERT INTO BSNGMatchScanning(MatchId, LiveHomeScore, LiveAwayScore, EventDate, EventStatus, CreatedTime, LastRejectedTicket
					, LastRejectedTime, ScannedTime)
	SELECT	m.MatchId
			, m.LiveHomeScore
			, m.LiveAwayScore
			, m.EventDate
			, m.EventStatus
			, @CurrentTime AS [CreatedTime]
			, NULL AS [LastRejectedTicket]
			, NULL AS [LastRejectedTime]
			, NULL AS [ScannedTime]			
	FROM	bodb02.dbo.Match m WITH (NOLOCK)
			INNER JOIN bodb02.dbo.League l WITH (NOLOCK)
				ON m.leagueid = l.leagueid
			LEFT JOIN dbo.BSNGMatchScanning bms WITH (NOLOCK) 
				ON m.matchid = bms.MatchId AND m.livehomescore = bms.LiveHomeScore AND m.liveAwayscore = bms.LiveAwayScore
	WHERE	m.kickofftime <= GETDATE()
			AND m.sporttype = 1
			AND l.IsMainMarket = 1
			AND l.isTest = 0
			AND bms.MatchId IS NULL;
	
	---Return Match to Scanned
	SELECT  DISTINCT bms.MatchId
	FROM	BSNGMatchScanning bms WITH (NOLOCK)
	WHERE	bms.CreatedTime = @CurrentTime;
	
END;
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_BSNG_PreCondition] TO [bodbSPUNet];
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_BSNG_PreCondition] TO [bodbSPUNet];
GO 
