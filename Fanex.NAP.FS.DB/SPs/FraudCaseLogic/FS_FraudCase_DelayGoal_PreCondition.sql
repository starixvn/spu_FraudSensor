
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
ALTER PROCEDURE [dbo].[FS_FraudCase_DelayGoal_PreCondition]

AS
/*
	Created: 20180312@CaseyHuynh
	Task : Get Match to scan for Delay Goal Fraud Case (Match Status = Running AND Sport = Soccer AND League Type = Main Markets AND Kick-off Time < Current Time)
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20180312@CaseyHuynh: Correct Code Syntax
		- 20180525@Casey.Huynh: Exclude League Test
		

	Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @fraudCaseID int = 8
	
	IF OBJECT_ID('tempdb..#tmpMatchRunning') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatchRunning;
	END;

	CREATE TABLE #tmpMatchRunning(
			MatchId			INT
		,	EventDate		DATE
		,	KickOffTime		DATETIME
		,	MaxTransid		BIGINT
	);

	INSERT INTO #tmpMatchRunning
	SELECT	[MatchId]		= m.matchid
		,	[EventDate]		= m.eventdate
		,	[KickOffTime]	= m.kickofftime
		,	[MaxTransid]	= max(b.transid)
	FROM bodb02.dbo.match AS m WITH(NOLOCK)
		INNER JOIN bodb02.dbo.League AS l WITH(NOLOCK) ON m.leagueid = l.leagueid
		INNER JOIN bodb02.dbo.Bettrans AS b WITH(NOLOCK) ON m.matchid=b.matchid
	WHERE m.sporttype = 1
		AND	m.eventstatus = 'Running'
		AND	m.kickofftime <= GETDATE()
		AND l.IsMainMarket = 1
		AND l.leaguename NOT LIKE '%TEST%'
	GROUP BY  m.matchid, m.eventdate, m.kickofftime;
 
	---Insert New Match to DelayGoalMatchScanning: LastScannedTransId default = 0 
	INSERT INTO dbo.DelayGoalMatchScanning (Matchid, EventDate, KickOffTime, fromTransId)	
	SELECT	[MatchId]		= mr.matchid
		,	[EventDate]		= mr.eventdate
		,	[KickOffTime]	= mr.kickofftime
		,	[fromTransid]	= 0
	FROM #tmpMatchRunning AS mr WITH(NOLOCK)
		LEFT JOIN dbo.DelayGoalMatchScanning AS ms WITH(NOLOCK)ON mr.matchid = ms.matchid
	WHERE ms.Matchid is NULL;
	
	---Return Match List Need to Scanning
	SELECT mr.Matchid
	FROM #tmpMatchRunning AS mr WITH(NOLOCK);
	
END;
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_DelayGoal_PreCondition] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_DelayGoal_PreCondition] TO [bodbSPUNet]
GO 
