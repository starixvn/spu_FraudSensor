USE bodb_FraudSensor
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
ALTER PROCEDURE dbo.FS_FraudCase_OfficeIP_Rule
	@MatchIds VARCHAR(MAX) = NULL
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

	DECLARE @fraudCaseID INT = 2;

	--===1. INITIAL TEMP #TABLEs===========
	--#tmpMatchInput: Store Scanned Matches with Max(transid) will be scanned
	IF OBJECT_ID('tempdb..#tmpMatchInput') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatchInput;
	END;

	CREATE TABLE #tmpMatchInput
	(
		MatchId		INT PRIMARY KEY
		, EventDate	DATETIME
	);
	
	--#tmpBettype Bettype will be scanned
	IF OBJECT_ID('tempdb..#tmpBettype') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpBettype;
	END;

	CREATE TABLE #tmpBettype
	(
		Bettype INT PRIMARY KEY
	);
	
	
	--#tmpFraudTickets: Store Fraud Ticket
	IF OBJECT_ID('tempdb..#tmpFraudTickets') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpFraudTickets;
	END;

	CREATE TABLE #tmpFraudTickets
	(
			CustId INT NOT NULL
		, 	MatchId INT NOT NULL
		, 	TransId	BIGINT NOT NULL
		, 	EventDate DATE	NULL
	);
	
	--===2. INSERT DATA TO #tmpMatchInput===========
	INSERT INTO #tmpMatchInput(MatchId)
	SELECT val FROM dbo.SPU_StrSplit(@MatchIds, ', ');
	
	UPDATE mi 
	SET		mi.EventDate = m.eventdate
	FROM	#tmpMatchInput AS mi WITH(ROWLOCK, UPDLOCK)
			INNER JOIN bodb02.dbo.Match m WITH (NOLOCK) ON mi.MatchId = m.matchid;
	
	--===3. INSERT DATA TO #tmpBettype===========
	INSERT INTO #tmpBettype(Bettype)
	SELECT DISTINCT Bettype FROM bodb02.dbo.SportBettypeMatrix  AS sm WITH(NOLOCK) WHERE sm.ProductID = 1;	

	--===4. INSERT DATE TO #tmpFraudTickets
	INSERT INTO #tmpFraudTickets(CustId, MatchId, TransId, EventDate)
	SELECT	  b.custid
			, b.matchid
			, b.transid
			, mi.EventDate
	FROM	bodb02.dbo.bettrans AS b WITH (NOLOCK)
			INNER JOIN #tmpMatchInput AS mi WITH (NOLOCK) ON mi.matchid = b.MatchId
			INNER JOIN #tmpBettype AS bt WITH (NOLOCK) ON b.bettype = bt.Bettype
			INNER JOIN dbo.OfficeIP AS oi WITH (NOLOCK) ON b.ip LIKE ''+ oi.IPRange +'.%' AND (CAST(PARSENAME(b.ip, 1) AS TINYINT) BETWEEN oi.FromNumber AND oi.ToNumber)
	WHERE	b.currency <> 20;
	
	--===5. INSERT SCANNED MATCH: TABLE "ScannedMatch" (ScannedDate, SuspiciousTicketsCount)===========		
	INSERT INTO dbo.ScannedMatch (Matchid, FraudCaseId, ScannedDate, SuspiciousTicketsCount) 
	SELECT [MatchId] = mi.matchid 
		, [FraudCaseId] = @fraudCaseID
		, [ScannedDate] = GETDATE() 
		, [SuspiciousTicketsCount] = (ISNULL((SELECT COUNT(1) 
											FROM #tmpFraudTickets t1 WITH (NOLOCK)
											WHERE t1.MatchId = mi.MatchId 
											GROUP BY t1.MatchId ), 0)) 
	FROM #tmpMatchInput AS mi WITH (NOLOCK) 
		LEFT JOIN dbo.ScannedMatch AS sm WITH (NOLOCK)ON mi.matchid = sm.matchid AND sm.FraudCaseId = @fraudCaseID
	WHERE sm.Matchid IS NULL AND sm.FraudCaseId IS NULL ; 
	
	IF EXISTS (SELECT TOP(1) 1 FROM #tmpFraudTickets WITH(NOLOCK))
	BEGIN							
		--===6. INSERT: TABLE "SuspiciousInfo" if (Match, Custid, FrauseCase) is Exitsing
		INSERT INTO	dbo.SuspiciousInfo(CustId, MatchId, EventDate, FraudCaseId, SuspiciousDate, TransIds, ViolatiONCounter, TicketCounter)
		SELECT	[CustId]			= ft.CustId
			, 	[MatchId]			= ft.MatchId
			, 	[EventDate]			= ft.EventDate
			, 	[FraudCaseId]		= @fraudCaseID
			, 	[SuspiciousDate]	= GETDATE()
			, 	[TransIds]			= STUFF((SELECT ', ', CONVERT(VARCHAR(15), t1.TransId) 
									 FROM #tmpFraudTickets AS t1 WITH(NOLOCK)
									 WHERE t1.CustId = ft.CustId AND t1.MatchId = ft.MatchId 
									 FOR XML PATH('')), 1, 1, '')
			, 	[ViolatiONCounter]	= 1
			, 	[TicketCounter]		= COUNT(1)
		FROM	#tmpFraudTickets AS ft WITH (NOLOCK)
				LEFT JOIN dbo.SuspiciousInfo AS si WITH (NOLOCK) 
					ON si.MatchId = ft.MatchId AND si.FraudCaseId = @fraudCaseID AND si.CustId = ft.CustId 
		WHERE	si.MatchID IS NULL AND si.FraudCaseId IS NULL AND si.CustId IS NULL
		GROUP BY ft.CustId, ft.MatchId, ft.EventDate;
			
		--===8. UPDATE: TABLE "SuspiciousCustomer" IsVerified=0 if has new Fraud Tickets)
		UPDATE	sc 
		SET		sc.IsVerified=0
		FROM	dbo.SuspiciousCustomer AS sc WITH (ROWLOCK, UPDLOCK)
		WHERE	Custid IN (SELECT DISTINCT(Custid)
							FROM #tmpFraudTickets  AS ft WITH (NOLOCK)
							WHERE sc.Custid = ft.CustId);

		--===9. INSERT: TABLE "SuspiciousCustomer" if NOT Exitsing Custid
		INSERT INTO dbo.SuspiciousCustomer(Custid, IsDelinquent, IsHedge, IsDirty, IsVerified)
		SELECT DISTINCT ft.CustId, 0, 0, 0, 0
		FROM #tmpFraudTickets  AS ft WITH (NOLOCK)
		WHERE ft.CustId NOT IN (
				SELECT sc.CustId 
				FROM dbo.SuspiciousCustomer  AS sc WITH (NOLOCK)
			);
	END;		
END;
GO 
 GRANT EXECUTE ON dbo.FS_FraudCase_OfficeIP_Rule TO bodbSPUNet;
GO 
 GRANT VIEW DEFINITION ON dbo.FS_FraudCase_OfficeIP_Rule TO bodbSPUNet;
