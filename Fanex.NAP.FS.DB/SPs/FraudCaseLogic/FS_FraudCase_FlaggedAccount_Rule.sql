
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_FlaggedAccount_Rule] 
		@MatchIds				VARCHAR(MAX) = NULL
	,	@FlaggedAccountsXML		VARCHAR(MAX)
AS
/*
	Created: 20180308@LONG
	Task : Rule for Flagged Account fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20180321@LONG: Support ViolatiONCounter & TicketCounter [Redmine ID: 92293]
		- 20180406@LONG: Get flagged accounts list directly from FPS's api
		- 20180312@CaseyHuynh: Correct Code Syntax
		- 20181114@Long: Split data in SuspiciousInfo by Currency [Redmine ID: #104707]

	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	

	DECLARE @CurrentFraudCaseId TINYINT;
	SET @CurrentFraudCaseId = 6; -- Flagged Account	
	DECLARE @idoc INT;
	
	IF OBJECT_ID('tempdb..#tmpFlaggedAccounts') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpFlaggedAccounts;
	END
	
	CREATE TABLE #tmpFlaggedAccounts(
			CustId				INT
		,	EvidenceCode		NVARCHAR(50)
		,	EvidenceName		NVARCHAR(50)
	);

	CREATE CLUSTERED INDEX	#CIX_tmpFlaggedAccountsIdx ON #tmpFlaggedAccounts (
			CustId				ASC
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
		,	Currency			INT
	);

	CREATE CLUSTERED INDEX	#CIX_tmpFraudTicketsIdx ON #tmpFraudTickets (
			CustId				ASC
		,	MatchId				ASC
		,	TransId				ASC
	);

	IF OBJECT_ID('tempdb..#tmpMatches') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatches;
	END;

	CREATE TABLE #tmpMatches
	(
		MatchId INT PRIMARY KEY
	);

	-- Insert flagged accounts	
	EXEC sp_xml_preparedocument @idoc OUTPUT, @FlaggedAccountsXML;

	INSERT INTO #tmpFlaggedAccounts(CustId)
	SELECT Data.CustId
	FROM	OPENXML (@idoc, '/Root/CustomerEvidence', 1)
			WITH (CustId INT, EvidenceCode NVARCHAR(50), EvidenceName NVARCHAR(50)) AS Data;

	INSERT INTO #tmpMatches(MatchId)
	SELECT val FROM SPU_StrSplit(@MatchIds, ',');

	INSERT INTO #tmpFraudTickets(CustId, MatchId, EventDate, TransId, Currency)
	SELECT	DISTINCT [CustId]	= ans.custid
		,	[MatchId]			= ans.matchid
		,	[EventDate]			= tch.EventDate
		,	[TransId]			= ans.transid
		,	[Currency]			= ans.currency
	FROM bodb02.dbo.match tch WITH (NOLOCK)
	INNER JOIN bodb02.dbo.bettrans ans WITH (NOLOCK) ON tch.matchid = ans.matchid
	INNER JOIN #tmpFlaggedAccounts fa WITH (NOLOCK) ON fa.custid = ans.custid
	WHERE tch.matchid IN (SELECT MatchId FROM #tmpMatches WITH (NOLOCK))
		AND ans.bettype IN (
			SELECT DISTINCT spb.BetType
			FROM bodb02.dbo.SportBettypeMatrix spb WITH (NOLOCK)
			WHERE spb.ProductID = 1
		)
		AND ans.custid NOT IN (SELECT CustId FROM dbo.DelayGoalAccount WITH (NOLOCK))
		AND ans.currency <> 20;

	-- UPDATE match info for ScannedMatch table
	DELETE FROM dbo.ScannedMatch WITH (ROWLOCK)
	WHERE FraudCaseId = @CurrentFraudCaseId
		AND MatchId IN (SELECT MatchId FROM #tmpMatches WITH (NOLOCK));

	INSERT INTO dbo.ScannedMatch(MatchId, FraudCaseId, ScannedDate, SuspiciousTicketsCount)
	SELECT	[MatchId]					= m.MatchId
		,	[FraudCaseId]				= @CurrentFraudCaseId
		,	[ScannedDate]				= GETDATE()
		,	[SuspiciousTicketsCount]	= COUNT(ft.TransId)
	FROM #tmpMatches m WITH (NOLOCK)
	LEFT JOIN #tmpFraudTickets ft WITH (NOLOCK) ON m.MatchId = ft.MatchId
	GROUP BY m.MatchId, ft.MatchId;

	-- Check the results and store/return data
	IF(SELECT COUNT(1) FROM #tmpFraudTickets WITH (NOLOCK)) > 0
	BEGIN
	
		UPDATE sc WITH (ROWLOCK)
		SET sc.IsVerified = 0 
		FROM dbo.SuspiciousCustomer sc 
		WHERE Custid IN (SELECT DISTINCT(Custid)
							FROM #tmpFraudTickets ft WITH (NOLOCK)
							WHERE sc.Custid = ft.CustId);
	
		INSERT INTO dbo.SuspiciousCustomer(Custid)
		SELECT DISTINCT ft.CustId
		FROM #tmpFraudTickets ft WITH (NOLOCK)
		WHERE ft.CustId NOT IN (
				SELECT sc.CustId 
				FROM dbo.SuspiciousCustomer sc WITH (NOLOCK)
			);
					
		DELETE FROM dbo.SuspiciousInfo WITH (ROWLOCK)
		WHERE FraudCaseId = @CurrentFraudCaseId
			AND MatchId IN (SELECT MatchId FROM #tmpMatches WITH (NOLOCK));

		INSERT INTO dbo.SuspiciousInfo(CustId, MatchId, EventDate, FraudCaseId, SuspiciousDate, TransIds, ViolatiONCounter, TicketCounter, CurrencyId)
		SELECT [CustId]			= ft.CustId
			,	[MatchId]			= ft.MatchId
			,	[EventDate]			= ft.EventDate
			,	[FraudCaseId]		= @CurrentFraudCaseId
			,	[SuspiciousDate]	= GETDATE()
			,	[TransIds]			= STUFF((SELECT ',', CONVERT(VARCHAR(15), t1.TransId) 
										 FROM #tmpFraudTickets AS t1 WITH (NOLOCK)
										 WHERE t1.CustId = ft.CustId AND t1.MatchId = ft.MatchId AND t1.Currency = ft.Currency
										 FOR XML PATH('')), 1, 1, '')
			,	[ViolatiONCounter]	= 1
			,	[TicketCounter]		= COUNT(ft.TransId)			
			,	[CurrencyId]		= ft.Currency
		FROM #tmpFraudTickets ft WITH (NOLOCK)
		GROUP BY ft.CustId, ft.MatchId, ft.EventDate, ft.Currency;		

	END;
END;

GO
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/