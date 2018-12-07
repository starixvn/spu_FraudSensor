GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_BetsNearGoal_Rule] 
	@MatchIds VARCHAR(MAX) = NULL
AS
/*
	Created: 20181207@Long
	Task : Rule for Bets Near The Goal fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:	
		- 20181207@Long: Created [Redmine ID: 106079]

	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	

	DECLARE @CurrentFraudCaseId TINYINT, @MinTicketsCount TINYINT;
	SET @CurrentFraudCaseId = 11; -- Bets Near Goal
	SET	@MinTicketsCount = 5;
	
	DECLARE @ScannedDate DATETIME;
	SET	@ScannedDate = GETDATE();
	
	-- Arrange tables and initialize
	IF OBJECT_ID('tempdb..#tmpBettype') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpBettype;
	END
	
	CREATE TABLE #tmpBettype(
		Bettype	INT
	);

	-- Arrange tables and initialize
	IF OBJECT_ID('tempdb..#tmpLastTickets') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpLastTickets;
	END

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
	-------------------------------------

	IF OBJECT_ID('tempdb..#tmpMatches') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatches;
	END

	CREATE TABLE #tmpMatches
	(
		MatchId			INT PRIMARY KEY
		, EventDate		DATETIME
		, EventStatus	NVARCHAR(50)
		, CurrentScore	TINYINT
	);
	-------GET BETTYPE--------------------------------
	INSERT INTO #tmpBettype (Bettype)
	SELECT DISTINCT bettype
	FROM	bodb02.dbo.SportBettypeMatrix WITH(NOLOCK)
	WHERE	SportType = 1 ;
	
	-------GET MATCH-----
	--------------------------------------------------
	INSERT INTO #tmpMatches(MatchId)
	SELECT val FROM dbo.SPU_StrSplit(@MatchIds, ',');
	
	
	--- Get Current Score of Match -- Only Scan for Score less than currentScore
	UPDATE	ms
	SET		ms.CurrentScore = m.livehomescore + liveawayscore, ms.EventDate = m.eventdate, ms.EventStatus = m.eventstatus
	FROM	#tmpMatches ms WITH(ROWLOCK)
			INNER JOIN bodb02.dbo.Match m WITH (NOLOCK) ON ms.MatchId = m.MatchId; 
	
	
	--- GET THE LAST REJECT TIME OF PREVIOUS SCORE		
	INSERT INTO dbo.BSNGScanInfo (MatchId, EventDate, EventStatus, LiveHomeScore, LiveAwayScore, LastRejectedTicket, LastRejectedTime, HasScanned)
	SELECT ms.MatchId
			, ms.EventDate
			, ms.EventStatus
			, b.livehomescore
			, b.liveawayscore
			, MAX(transid) AS [TransId]
			, MAX(TransDate) AS [DeleteTime]	
			, 0	
	FROM	bodb02.dbo.bettrans b WITH(NOLOCK)
			INNER JOIN #tmpBettype bt WITH(NOLOCK) ON b.bettype = bt.Bettype
			INNER JOIN #tmpMatches ms WITH(NOLOCK) ON b.matchid = ms.MatchId
			LEFT JOIN dbo.BSNGScanInfo si WITH(NOLOCK) ON si.MatchId = ms.MatchId AND si.livehomescore = b.livehomescore AND si.LiveAwayScore = b.liveawayscore
	WHERE	b.liveindicator = 1
			AND b.statusID = 101
			AND (b.livehomescore + b.liveawayscore) < ms.CurrentScore
			AND si.MatchId IS NULL
	GROUP BY	ms.MatchId
				, ms.EventDate
				, ms.EventStatus
				, b.livehomescore
				, b.liveawayscore;
	
	
	--- GET FRAUD TICKET
	INSERT INTO #tmpFraudTickets (CustId, MatchId, EventDate, TransId, LiveHomeScore, LiveAwayScore)
	SELECT  ans.custid
		,	ans.matchid
		,	tch.eventdate
		,	ans.transid
		,	ans.liveawayscore
		,	ans.livehomescore
	FROM bodb02.dbo.match AS tch WITH (NOLOCK)
	INNER JOIN bodb02.dbo.bettrans AS ans WITH (NOLOCK) ON tch.matchid = ans.matchid
	INNER JOIN dbo.BSNGScanInfo AS si WITH(NOLOCK) ON si.matchid = ans.matchid
	INNER JOIN #tmpMatches AS tm WITH(NOLOCK) ON tm.MatchId = si.MatchId
	WHERE  tch.matchid IN (SELECT MatchId FROM #tmpMatches WITH (NOLOCK))
		AND ans.liveindicator = 1
		AND ans.transdate BETWEEN DATEADD(ss, -90, si.LastRejectedTime) AND si.LastRejectedTime
		AND ans.livehomescore = si.livehomescore
		AND ans.liveawayscore = si.liveawayscore
		AND ans.bettype IN (1, 3, 5, 7, 8, 15)
		AND ans.currency NOT IN (20, 27, 28)
		AND ans.statusID = 112 -- WON tickets
		AND si.HasScanned = 0
	GROUP BY ans.custid
		,	ans.username
		,	ans.matchid
		,	tch.eventdate
		,	ans.liveawayscore
		,	ans.livehomescore
	HAVING COUNT(ans.transid) > @MinTicketsCount;
	
	--===4. UPDATE: TABLE "ScannedMatch" (ScannedDate,SuspiciousTicketsCount)===========	
	UPDATE sm WITH (ROWLOCK, UPDLOCK)
	SET sm.ScannedDate = GETDATE()
		,sm.SuspiciousTicketsCount = sm.SuspiciousTicketsCount + ISNULL((SELECT COUNT(1) 
																			FROM #tmpFraudTickets t1 WITH (NOLOCK)
																			WHERE t1.MatchId = mi.MatchId 
																			GROUP BY t1.MatchId ),0)
	FROM dbo.ScannedMatch sm
			INNER JOIN #tmpMatches mi WITH (NOLOCK) ON sm.MatchId = mi.MatchId
	WHERE sm.FraudCaseId = @CurrentFraudCaseId;
	
	--===5. INSERT SCANNED MATCH: TABLE "ScannedMatch" (ScannedDate,SuspiciousTicketsCount)===========		
	INSERT INTO dbo.ScannedMatch (Matchid, FraudCaseId, ScannedDate, SuspiciousTicketsCount)  
	SELECT [MatchId] = mi.matchid 
		, [FraudCaseId] = @CurrentFraudCaseId
		, [ScannedDate] = GETDATE() 
		, [SuspiciousTicketsCount] = (ISNULL((SELECT COUNT(1) 
											FROM #tmpFraudTickets t1 WITH (NOLOCK)
											WHERE t1.MatchId = mi.MatchId 
											GROUP BY t1.MatchId ),0)) 
	FROM #tmpMatches mi WITH (NOLOCK) 
		LEFT JOIN dbo.ScannedMatch sm WITH (NOLOCK)ON mi.matchid = sm.matchid AND sm.FraudCaseId = @CurrentFraudCaseId
	WHERE sm.Matchid IS NULL AND sm.FraudCaseId IS NULL ; 

	-- Check the results and store/return data
	IF EXISTS (SELECT TOP(1) 1 FROM #tmpFraudTickets WITH (NOLOCK))
	BEGIN
		--===6. UPDATE: TABLE "SuspiciousInfo" if (Match,Custid,FrauseCase) is Exitsing
		UPDATE si WITH (ROWLOCK, UPDLOCK)
		SET si.SuspiciousDate	= GETDATE()
				,	si.TransIds	= si.TransIds + ','+ STUFF((SELECT ',', CONVERT(VARCHAR(15), t1.TransId) 
										 FROM #tmpFraudTickets t1 WITH (NOLOCK)
										 WHERE t1.CustId = ft.CustId AND t1.MatchId = ft.MatchId AND si.FraudCaseId = @CurrentFraudCaseId
										 FOR XML PATH('')), 1, 1, '')
				,	si.TicketCounter = si.TicketCounter + (SELECT COUNT(1) 
															FROM #tmpFraudTickets t1 WITH (NOLOCK)
															WHERE t1.CustId = ft.CustId AND t1.MatchId = ft.MatchId AND si.FraudCaseId = @CurrentFraudCaseId)
				,	si.IsVerified = 0
				,	si.ViolationCounter = si.ViolationCounter + (SELECT COUNT(1)
										FROM (SELECT t2.LiveHomeScore,t2.LiveAwayScore
										FROM #tmpFraudTickets AS t2 WITH (NOLOCK)
										WHERE t2.CustId = ft.CustId AND t2.MatchId = ft.MatchId
										GROUP BY t2.LiveHomeScore,t2.LiveAwayScore) AS A)
		FROM dbo.SuspiciousInfo si 
				INNER JOIN #tmpFraudTickets ft WITH (NOLOCK)
					ON si.MatchId = ft.MatchId AND si.FraudCaseId = @CurrentFraudCaseId AND si.CustId = ft.CustId;
							
		--===7. INSERT: TABLE "SuspiciousInfo" if (Match,Custid,FrauseCase) is Exitsing
		INSERT INTO dbo.SuspiciousInfo(CustId, MatchId, EventDate, FraudCaseId, SuspiciousDate, TransIds, ViolatiONCounter, TicketCounter)
			SELECT [CustId]			= ft.CustId
				,	[MatchId]			= ft.MatchId
				,	[EventDate]			= ft.EventDate
				,	[FraudCaseId]		= @CurrentFraudCaseId
				,	[SuspiciousDate]	= GETDATE()
				,	[TransIds]			= STUFF((SELECT ',', CONVERT(VARCHAR(15), t1.TransId) 
										 FROM #tmpFraudTickets AS t1 WITH(NOLOCK)
										 WHERE t1.CustId = ft.CustId AND t1.MatchId = ft.MatchId 
										 FOR XML PATH('')), 1, 1, '')
				,	ViolationCounter = (SELECT COUNT(1)
										FROM (SELECT t2.LiveHomeScore,t2.LiveAwayScore
										FROM #tmpFraudTickets AS t2 WITH (NOLOCK)
										WHERE t2.CustId = ft.CustId AND t2.MatchId = ft.MatchId
										GROUP BY t2.LiveHomeScore,t2.LiveAwayScore) AS A)
				,	[TicketCounter]		= COUNT(1)
			FROM #tmpFraudTickets ft WITH (NOLOCK)
					LEFT JOIN dbo.SuspiciousInfo si WITH (NOLOCK) 
						ON si.MatchId = ft.MatchId AND si.FraudCaseId = @CurrentFraudCaseId AND si.CustId = ft.CustId 
			WHERE si.MatchID IS NULL AND si.FraudCaseId IS NULL AND si.CustId IS NULL
			GROUP BY ft.CustId, ft.MatchId, ft.EventDate;
		
	
		--===8. UPDATE: TABLE "SuspiciousCustomer" IsVerified=0 if has new Fraud Tickets)
			UPDATE sc WITH (ROWLOCK, UPDLOCK)
			SET sc.IsVerified=0
			FROM dbo.SuspiciousCustomer sc 
			WHERE Custid IN (SELECT DISTINCT(Custid)
								FROM #tmpFraudTickets ft WITH (NOLOCK)
								WHERE sc.Custid = ft.CustId);

		--===9. INSERT: TABLE "SuspiciousCustomer" if NOT Exitsing Custid
			INSERT INTO dbo.SuspiciousCustomer(Custid,IsDelinquent,IsHedge,IsDirty,IsVerified)
			SELECT DISTINCT ft.CustId, 0,0,0,0
			FROM #tmpFraudTickets ft WITH (NOLOCK)
			WHERE ft.CustId NOT IN (
					SELECT sc.CustId 
					FROM dbo.SuspiciousCustomer sc WITH (NOLOCK)
				);
	END;
	
	UPDATE si
	SET		si.HasScanned = 1, si.ScannedDate = @ScannedDate
	FROM	dbo.BSNGScanInfo si WITH(ROWLOCK)
			INNER JOIN #tmpMatches tm WITH(NOLOCK) ON si.MatchId = tm.matchid
	WHERE	si.HasScanned = 0;
	
END
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_BSNG_Rule] TO [bodbSPUNet]
GO
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/