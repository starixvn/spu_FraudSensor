USE [bodb_FraudSensor]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_BSNG_Rule] 
	@MatchIds VARCHAR(MAX) = NULL
AS
/*
	Created: 20180306@Long
	Task : Rule for Big Stake Near The Goal fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:	
		- 20180321@Long: Support ViolationCounter & TicketCounter [Redmine ID: 92293]
		- 20180316@Casey: 
			+ Update get Oddslog from bodb_FraudSensor instead of "SPU_FraudDetection"
			+ Update Calculate Violation BSNG Rule: count 1 per each score
		- 20180406@Long: remove ticket status to scan for running matches
		- 20180312@CaseyHuynh: Correct Code Syntax

	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	

	DECLARE @CurrentFraudCaseId TINYINT;
	SET @CurrentFraudCaseId = 1; -- BSNG

	-- Arrange tables and initialize
	IF OBJECT_ID('tempdb..#tmpLastTickets') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpLastTickets;
	END
	
	CREATE TABLE #tmpLastTickets(
			MatchId				INT
		,	TransId				BIGINT
		,	DeleteTime			DATETIME
		,	LiveHomeScore		SMALLINT
		,	LiveAwayScore		SMALLINT
	);

	CREATE CLUSTERED INDEX	#CIX_tmpLastTicketsIdx ON #tmpLastTickets (
			MatchId				ASC
		,	TransId				ASC
	);

	IF OBJECT_ID('tempdb..#tmpFraudTickets') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpFraudTickets;
	END
	
	CREATE TABLE #tmpFraudTickets(
			CustId				INT
		,	MatchId				INT
		,	EventDate			DATETIME
		,	TransId				BIGINT
		,	LiveHomeScore		SMALLINT
		,	LiveAwayScore		SMALLINT
	);

	CREATE CLUSTERED INDEX	#CIX_tmpFraudTicketsIdx ON #tmpFraudTickets (
			CustId				ASC
		,	MatchId				ASC
		,	TransId				ASC
	);

	IF OBJECT_ID('tempdb..#tmpMatches') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatches;
	END

	CREATE TABLE #tmpMatches
	(
		MatchId INT PRIMARY KEY
	);

	INSERT INTO #tmpMatches(MatchId)
	SELECT val FROM SPU_StrSplit(@MatchIds, ',');
		
	-- Run the Rule
	INSERT INTO #tmpLastTickets (MatchId, TransId, DeleteTime, LiveHomeScore, LiveAwayScore)
	SELECT	[MatchId]			= ans.matchid
		,	[TransId]			= MAX(ans.transid)
		,	[DeleteTime]		= MAX(ans.transdate)
		,	[LiveHomeScore]		= ans.livehomescore
		,	[LiveAwayScore]		= ans.liveawayscore
	FROM bodb02.dbo.bettrans AS ans WITH (NOLOCK)
	WHERE ans.matchid IN (SELECT MatchId FROM #tmpMatches WITH (NOLOCK))
		AND ans.statusID = 101
		AND ans.liveindicator = 1
		AND ans.bettype IN (
				SELECT DISTINCT spb.BetType
				FROM bodb02.dbo.SportBettypeMatrix spb WITH (NOLOCK)
				WHERE spb.ProductID = 1
				)
	GROUP BY ans.livehomescore
		,	ans.liveawayscore
		,	ans.matchid;

	INSERT INTO #tmpFraudTickets (CustId, MatchId, EventDate, TransId, LiveHomeScore, LiveAwayScore)
	SELECT  [CustId]	= ans.custid
		,	[MatchId]			= ans.matchid
		,	[EventDate]			= ans.eventdate
		,	[TransId]			= ans.transid
		,	[LiveHomeScore]		= ans.liveawayscore
		,	[LiveAwayScore]		= ans.livehomescore
	FROM (
		SELECT  ans.custid
			,	ans.username
			,	ans.matchid
			,	tch.eventdate
			,	ans.transid
			,	ans.liveawayscore
			,	ans.livehomescore
			,	SUM(ans.stake * ans.actualrate) OVER (
					PARTITION BY ans.custid
					,	ans.matchid
					,	ans.bettype
					,	ans.liveawayscore
					,	ans.livehomescore
				) AS TotalStake
			,	40 * 0.01 * COALESCE(dds.maxbet, 0) AS MaxBet
		FROM bodb02.dbo.match AS tch WITH (NOLOCK)
		INNER JOIN bodb02.dbo.bettrans AS ans WITH (NOLOCK) ON tch.matchid = ans.matchid
		INNER JOIN #tmpLastTickets AS msc WITH(NOLOCK) ON msc.matchid = ans.matchid
		CROSS APPLY (
			SELECT TOP(1) l.Maxbet
			FROM bodb_FraudSensor.dbo.OddsLog AS l WITH (NOLOCK)
			WHERE l.oddsid = ans.oddsid
				AND l.logdate < ans.transdate
			ORDER BY l.logdate DESC
		) dds
		WHERE  tch.matchid IN (SELECT MatchId FROM #tmpMatches WITH (NOLOCK))
			AND ans.liveindicator = 1
			AND ans.transdate BETWEEN DATEADD(ss, -90, msc.deletetime) AND msc.deletetime
			AND ans.livehomescore = msc.livehomescore
			AND ans.liveawayscore = msc.liveawayscore
			AND ans.bettype IN (1, 3, 5, 7, 8, 15)
			AND ans.currency <> 20
		GROUP BY ans.custid
			,	ans.username
			,	ans.matchid
			,	tch.eventdate
			,	ans.transid
			,	ans.liveawayscore
			,	ans.livehomescore
			,	dds.maxbet
			,	ans.bettype
			,	ans.stake
			,	ans.actualrate
		) ans
	WHERE ans.TotalStake >= ans.Maxbet;

	-- Update match info for ScannedMatch table
	DELETE FROM dbo.ScannedMatch
	WHERE FraudCaseId = @CurrentFraudCaseId
		AND MatchId IN (SELECT MatchId FROM #tmpMatches WITH (NOLOCK));

	INSERT INTO dbo.ScannedMatch(MatchId, FraudCaseId, ScannedDate, SuspiciousTicketsCount)
	SELECT  [MatchId]					= m.MatchId
		,	[FraudCaseId]				= @CurrentFraudCaseId
		,	[ScannedDate]				= GETDATE()
		,	[SuspiciousTicketsCount]	= COUNT(ft.TransId)
	FROM #tmpMatches AS m WITH (NOLOCK)
	LEFT JOIN #tmpFraudTickets AS ft WITH (NOLOCK) ON m.MatchId = ft.MatchId
	GROUP BY m.MatchId, ft.MatchId;

	-- Check the results and store/return data
	IF EXISTS (SELECT TOP(1) 1 FROM #tmpFraudTickets WITH (NOLOCK))
	BEGIN
		UPDATE sc WITH (ROWLOCK)
		SET sc.IsVerified = 0 
		FROM dbo.SuspiciousCustomer AS sc 
		WHERE Custid IN (SELECT DISTINCT(Custid)
							FROM #tmpFraudTickets AS ft WITH (NOLOCK)
							WHERE sc.Custid = ft.CustId);
	
		INSERT INTO dbo.SuspiciousCustomer(Custid)
		SELECT DISTINCT ft.CustId
		FROM #tmpFraudTickets AS ft WITH (NOLOCK)
		WHERE ft.CustId NOT IN (
				SELECT sc.CustId 
				FROM dbo.SuspiciousCustomer AS sc WITH (NOLOCK)
			);

		DELETE FROM dbo.SuspiciousInfo WITH (ROWLOCK)
		WHERE FraudCaseId = @CurrentFraudCaseId
			AND MatchId IN (SELECT MatchId FROM #tmpMatches WITH (NOLOCK));

		INSERT INTO dbo.SuspiciousInfo(CustId, MatchId, EventDate, FraudCaseId, SuspiciousDate, TransIds, ViolationCounter, TicketCounter)
		SELECT  [CustId]			= ft.CustId
			,	[MatchId]			= ft.MatchId
			,	[EventDate]			= ft.EventDate
			,	[FraudCaseId]		= @CurrentFraudCaseId
			,	[SuspiciousDate]	= GETDATE()
			,	[TransIds]			= STUFF((SELECT ',', CONVERT(VARCHAR(15), t1.TransId) 
										 FROM #tmpFraudTickets AS t1 WITH (NOLOCK)
										 WHERE t1.CustId = ft.CustId AND t1.MatchId = ft.MatchId  
										 FOR XML PATH('')), 1, 1, '')
			,	[ViolationCounter]	= (SELECT COUNT(1)
										FROM (SELECT t2.LiveHomeScore,t2.LiveAwayScore
										FROM #tmpFraudTickets AS t2 WITH (NOLOCK)
										WHERE t2.CustId = ft.CustId AND t2.MatchId = ft.MatchId
										GROUP BY t2.LiveHomeScore,t2.LiveAwayScore) AS A)
			,	[TicketCounter]		= COUNT(ft.TransId)
		FROM #tmpFraudTickets AS ft WITH (NOLOCK)
		GROUP BY ft.CustId, ft.MatchId, ft.EventDate;
	END;
END
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_BSNG_Rule] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_BSNG_Rule] TO [bodbSPUNet]
GO 
