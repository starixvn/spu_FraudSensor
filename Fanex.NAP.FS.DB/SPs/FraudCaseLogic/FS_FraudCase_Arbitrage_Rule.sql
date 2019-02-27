/****** Object:  StoredProcedure [dbo].[FS_FraudCase_Arbitrage_Rule_LongEnhance]    Script Date: 11/15/2018 9:21:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[FS_FraudCase_Arbitrage_Rule] 
		@Matches			VARCHAR(MAX) = NULL -- xml format
	,	@Associations		VARCHAR(MAX) = NULL -- xml format
AS
/*
	Created: 20180816@Long
	Task : Rule for Arbitrage fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20180815@LONG: Implement Rule for Arbitrage fraud case
		- 20180924@LONG: Remove the XML Documents after usage
		- 20180926@LONG: Remove redundant fraud tickets [Redmine ID: 99753]
		- 20180926@LONG: Fix issue of livehomescore [Redmine ID: 000000]
		- 20180926@LONG: Performance tunning [Redmine ID: 000000]
		- 20180926@CASEY: Implement IP Rule [Redmine ID: 109092]
	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	

	-- ARRANGE
	DECLARE @CurrentFraudCaseId		TINYINT
		,	@MaxStakeToBeChecked	MONEY;
	SET		@CurrentFraudCaseId		= 10; -- Arbitrage
	SET		@MaxStakeToBeChecked	= 500;
	
	DECLARE @ScannedDate		DATETIME;
	SET	@ScannedDate			= GETDATE();
	
	IF OBJECT_ID('tempdb..#tmpMatches') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatches;
	END

	CREATE TABLE #tmpMatches
	(
			MatchId				INT
		,	CustId				INT
		,	BetType				INT
		,	Handicap			SMALLMONEY
		,	LiveHomeScore		INT
		,	LiveAwayScore		INT
	);

	CREATE CLUSTERED INDEX	#CIX_tmpMatches_MatchIdCustIdBettype ON #tmpMatches (
			MatchId				ASC
		,	CustId				ASC
		,	BetType				ASC		
		,	LiveHomeScore		ASC
		,	LiveAwayScore		ASC
	);

	CREATE NONCLUSTERED INDEX	#NCIX_tmpMatches_MatchId ON #tmpMatches (
			MatchId				ASC
	);


	IF	OBJECT_ID('tempdb..#tmpAssociation') IS NOT NULL
	BEGIN;
		DROP TABLE #tmpAssociation;
	END;

	CREATE TABLE	#tmpAssociation (
			[CustId]			INT NOT NULL PRIMARY KEY
		,	[GroupId]			INT NOT NULL
	);

	CREATE NONCLUSTERED INDEX	#NCIX_tmpAssociation_GroupID ON #tmpAssociation (
			[GroupId]			ASC
	);

	IF	OBJECT_ID('tempdb..#tmpFraudIP') IS NOT NULL
	BEGIN;
		DROP TABLE #tmpFraudIP;
	END;

	CREATE TABLE	#tmpFraudIP (
			[MatchId]			INT
		,	[TotalLiveScore]	INT
		,	[Bettype]			INT
		,	[Handicap]			SMALLMONEY
		,	[IP]				BIGINT
	);

	IF	OBJECT_ID('tempdb..#tmpMatchList') IS NOT NULL
	BEGIN;
		DROP TABLE #tmpMatchList;
	END;
	CREATE TABLE #tmpMatchList (MatchId INT)


	IF OBJECT_ID('tempdb..#tmpBettrans') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpBettrans;
	END

	CREATE TABLE #tmpBettrans
	(
			CustId				INT
		,	Username			NVARCHAR(50)
		,	CurrencyId			TINYINT
		,	CurrencyName		NVARCHAR(50)
		,	TransId				BIGINT
		,	MatchId				INT
		,	Bettype				INT
		,	TotalLiveScore		INT
		,	Betteam				NVARCHAR(50)
		,	Handicap			SMALLMONEY
		,	Stake				MONEY
		,	EventDate			DATETIME
		,	IP					BIGINT
	);

	CREATE NONCLUSTERED INDEX	#NCIX_tmpBettrans_CustId ON #tmpBettrans (
			CustId				ASC
	);

	IF OBJECT_ID('tempdb..#tmpBettransAssociation') IS NOT NULL
    BEGIN
        DROP TABLE  #tmpBettransAssociation;
    END

	CREATE TABLE #tmpBettransAssociation
	(
			CustId				INT
		,	Username			NVARCHAR(50)
		,	CurrencyId			TINYINT
		,	CurrencyName		NVARCHAR(50)
		,	TransId				BIGINT
		,	MatchId				INT
		,	Bettype				INT
		,	TotalLiveScore		INT
		,	Betteam				NVARCHAR(50)
		,	Handicap			SMALLMONEY
		,	Stake				MONEY
		,	EventDate			DATETIME
		,	Ip					BIGINT
		,	GroupId				INT
		,	MergeGroupId		INT
		,	IsIPFraud			BIT		
	);

	CREATE NONCLUSTERED INDEX	#NCIX_tmpBettransAssociation ON #tmpBettransAssociation(
			
			MatchId				ASC
		,   CustId				ASC
		,	Bettype				ASC
		,	TotalLiveScore		ASC		
		,	Handicap			ASC
		,	Betteam				ASC
		,	GroupId				ASC
	);

	CREATE NONCLUSTERED INDEX	#NCIX_tmpBettransAssociation_IP ON #tmpBettransAssociation(IP ASC);

	IF OBJECT_ID('tempdb..#tmpIPGroup') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpIPGroup;
	END

	CREATE TABLE #tmpIPGroup
	(		
			MatchId				INT	
		,	TotalLiveScore		INT
		,	Bettype				INT
		,	Handicap			SMALLMONEY
		,	IP					BIGINT
		,	MergeGroupId		INT
		,	MinGroupId			INT	
	);
	
	CREATE CLUSTERED INDEX	#CIX_#tmpIPGroup_MatchIdScoreBettype ON #tmpIPGroup (
			MatchId				ASC
		,	TotalLiveScore		ASC
		,	BetType				ASC		
		,	Handicap			ASC
		,	IP		ASC
	);

	IF OBJECT_ID('tempdb..#tmpArbitrageGroups') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpArbitrageGroups;
	END

	CREATE TABLE #tmpArbitrageGroups
	(
			MatchId				INT PRIMARY KEY
		,	GroupId				INT
	);

	CREATE NONCLUSTERED INDEX	#NCIX_tmpArbitrageGroups_GroupID ON #tmpArbitrageGroups (
			[GroupId]			ASC
	);

	IF OBJECT_ID('tempdb..#tmpFraudTickets') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpFraudTickets;
	END
	
	CREATE TABLE #tmpFraudTickets(
			CustId				INT
		,	Username			NVARCHAR(50)
		,	CurrencyId			TINYINT
		,	CurrencyName		NVARCHAR(50)
		,	MatchId				INT
		,	GroupId				INT
		,	Bettype				INT
		,	EventDate			DATETIME
		,	TransId				BIGINT
		,	TotalLiveScore		INT
		,	Handicap			SMALLMONEY
		,   Ip					BIGINT
		,	IsIPFraud			BIT
		,	MergeGroupId		INT
	);

	CREATE CLUSTERED INDEX	#CIX_tmpFraudTicketsIdx ON #tmpFraudTickets (
			MatchId				ASC
		,	Bettype				ASC
		,	Handicap			ASC
		,	TotalLiveScore		ASC
		,	CustId				ASC
		,	GroupId				ASC
	);

	CREATE NONCLUSTERED INDEX	#NCIX_tmpFraudTicketsIdx ON #tmpFraudTickets (
			MatchId				ASC
		,	CustId				ASC
	);

	IF OBJECT_ID('tempdb..#tmpMatchBettypeBetteamSumStake') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatchBettypeBetteamSumStake;
	END
	
	CREATE TABLE #tmpMatchBettypeBetteamSumStake(
			MergeGroupId		INT
		,	MatchId				INT
		,	Bettype				INT
		,	TotalLiveScore		INT
		,	Handicap			SMALLMONEY		
		,	TotalStakeH			MONEY
		,	TotalStakeA			MONEY
	);

	CREATE CLUSTERED INDEX	#CIX_tmpMatchBettypeBetteamSumStakeIdx ON #tmpMatchBettypeBetteamSumStake (
			MergeGroupId		ASC
		,	MatchId				ASC
		,	Bettype				ASC
		,	Handicap			ASC
		,	TotalLiveScore		ASC
	);

	-- INIT	
	DECLARE @idocMatch INT
		,	@idocAssociation INT;

	--Create an internal representation of the XML document
	EXEC sp_xml_preparedocument @idocMatch OUTPUT, @Matches;
	EXEC sp_xml_preparedocument @idocAssociation OUTPUT, @Associations;

	-- Execute a SELECT statement that uses the OPENXML rowset provider
	
	INSERT INTO #tmpMatches (MatchId, CustId, BetType, Handicap, LiveHomeScore, LiveAwayScore)
	SELECT	Data.MatchId
		,	Data.CustId
		,	Data.BetType
		,	Data.Handicap
		,	Data.LiveHomeScore
		,	Data.LiveAwayScore
	FROM OPENXML(@idocMatch, '/Root/Match', 2) WITH (
			MatchId INT '@MatchId'
		,	BetType INT	'@BetType'
		,	CustId INT '@CustId'
		,	Handicap MONEY '@Handicap'
		,	LiveHomeScore INT '@LiveHomeScore'
		,	LiveAwayScore INT '@LiveAwayScore'
	) AS Data;

	-- Execute a SELECT statement that uses the OPENXML rowset provider
	INSERT INTO #tmpAssociation (CustId, GroupId)
	SELECT	Data.CustId
		,	Data.GroupId
	FROM OPENXML(@idocAssociation, '/Root/Association', 2) WITH (
			CustId INT '@CustId'	
		,	GroupId INT '@GroupId'
	) AS Data;

	--================GET: BETTRANS TICKET
	INSERT INTO #tmpBettrans(CustId, Username, CurrencyId, CurrencyName, MatchId, TransId, Bettype, TotalLiveScore, Betteam, Handicap, Stake, EventDate, IP)
	SELECT	DISTINCT [CustId]	= b.custid
		,	[Username]			= b.Username
		,	[CurrencyId]		= b.currency
		,	[CurrencyName]		= b.CurrencyName
		,	[MatchId]			= b.matchid
		,	[TransId]			= b.transid
		,	[Bettype]			= b.bettype
		,	[TotalLiveScore]	= (b.livehomescore + b.liveawayscore)
		,	[Betteam]			= b.betteam
		,	[Handicap]			= (b.hdp1 - b.hdp2)
		,	[stake]				= (b.stake * b.actualrate)
		,	[EventDate]			= b.winlostdate
		,	[IP]				= ISNULL(dbo.IPAddressToInteger(b.IP),0)
	FROM	bodb02.dbo.bettrans AS b WITH(NOLOCK)
			INNER JOIN #tmpMatches AS m WITH(NOLOCK) ON b.matchid = m.MatchId AND b.bettype = m.BetType	AND b.custid = m.CustId
					AND (b.hdp1 - b.hdp2) = m.Handicap AND b.livehomescore = m.LiveHomeScore AND b.liveawayscore = m.LiveAwayScore
	WHERE	b.matchId > 0
			AND b.bettype IN (1, 3, 7, 8)
			AND b.Currency NOT IN (20, 27, 28);

	---===========GET: ASSOCIATION TICKETS
	INSERT INTO #tmpBettransAssociation(CustId, Username, CurrencyId, CurrencyName, MatchId, TransId, Bettype, TotalLiveScore, Betteam, Handicap, Stake, EventDate, Ip, GroupId, MergeGroupId, IsIPFraud)
	SELECT	DISTINCT b.CustId
			, b.Username
			, b.CurrencyId
			, b.CurrencyName
			, b.MatchId
			, b.TransId
			, b.Bettype
			, b.TotalLiveScore
			, b.Betteam
			, b.Handicap
			, b.Stake
			, b.EventDate
			, b.Ip
			, a.GroupId AS GroupID
			, a.GroupId AS MergeGroupID
			, 0	AS IsIPFraud
	FROM	#tmpBettrans AS b WITH(NOLOCK)
			INNER JOIN #tmpAssociation AS a WITH(NOLOCK) ON b.CustId = a.CustId;

	---- Merge Group By IP
		--Find IP has more than one groupId

	Declare @Matchid INT

	INSERT INTO #tmpMatchList(MatchId)
	SELECT DISTINCT MatchId
	FROM #tmpMatches
	
	SELECT @Matchid = (SELECT TOP(1) Matchid FROM #tmpMatchList WITH(NOLOCK))


	WHILE (@Matchid IS NOT NULL)
	BEGIN
			
		DELETE #tmpIPGroup;
		
		INSERT INTO #tmpIPGroup(MatchId, TotalLiveScore, bettype, handicap, IP, MergeGroupId, MinGroupId)
		SELECT	DISTINCT a.MatchId, a.TotalLiveScore, a.bettype, a.handicap, a.IP, a.MergeGroupId, a.MinGroupId	
		FROM	(SELECT		t.matchid
							, t.TotalLiveScore
							, t.Bettype
							, t.Handicap
							, t.Ip
							, t.MergeGroupId
							, MIN(t.MergeGroupId) OVER (PARTITION BY t.MatchID, t.Bettype, t.TotalLiveScore, t.Handicap, t.IP ) AS MinGroupId
				FROM		#tmpBettransAssociation t WITH(NOLOCK)
				WHERE		t.Matchid = @Matchid
				GROUP BY	t.MatchID, t.Bettype, t.TotalLiveScore, t.Handicap, t.IP, t.MergeGroupId) a
		WHERE	a.MergeGroupId != a.MinGroupId ;

		IF EXISTS (SELECT TOP(1) (1) FROM #tmpIPGroup WITH(NOLOCK))
		BEGIN		
			UPDATE	t
			SET		t.MergeGroupId = g.MinGroupId
			FROM	#tmpBettransAssociation t WITH(ROWLOCK)
					INNER JOIN #tmpIPGroup g WITH(NOLOCK) ON g.MatchId = t.MatchId AND g.TotalLiveScore = t.TotalLiveScore
								AND g.Handicap = t.Handicap AND g.IP = t.IP
			WHERE	t.Matchid = @Matchid;
		END;

		DELETE #tmpMatchList
		WHERE MatchId = @Matchid

		SELECT @Matchid = (SELECT TOP(1) Matchid FROM #tmpMatchList WITH(NOLOCK))	

	END;
	--=================================================================================================	
	INSERT INTO #tmpMatchBettypeBetteamSumStake(MergeGroupId, MatchId, Bettype, Handicap, TotalLiveScore, TotalStakeH, TotalStakeA)
	SELECT DISTINCT ba.MergeGroupId
		,	ba.MatchId
		,	ba.Bettype
		,	ba.Handicap
		,	ba.TotalLiveScore
		,	SUM(CASE WHEN ba.Betteam = 'h' then ba.stake ELSE 0 END) AS TotalStakeH -- SumH.TotalStakeH
		,	SUM(CASE WHEN ba.Betteam = 'a' then ba.stake ELSE 0 END) AS TotalStakeA -- SumA.TotalStakeA
	FROM	#tmpBettransAssociation ba WITH(NOLOCK)
	GROUP BY ba.MergeGroupId, ba.MatchId, ba.Bettype, ba.TotalLiveScore, ba.Handicap
	HAVING COUNT(DISTINCT ba.Betteam) > 1;
	
	-- Get fraud tickets
	INSERT INTO #tmpFraudTickets (CustId, Username, CurrencyId, CurrencyName, MatchId, GroupId, Bettype, EventDate, TransId, TotalLiveScore, Handicap, IsIPFraud, IP, MergeGroupId )
	SELECT DISTINCT [CustId]			= bb.CustId
		,	[Username]			= bb.Username
		,	[CurrencyId]		= bb.CurrencyId
		,	[CurrencyName]		= bb.CurrencyName
		,	[MatchId]			= r.matchid
		,	[GroupId]			= bb.GroupId
		,	[Bettype]			= bb.Bettype
		,	[EventDate]			= bb.EventDate
		,	[TransId]			= bb.TransId
		,	[TotalLiveScore]	= r.TotalLiveScore
		,	[Handicap]			= r.Handicap
		,	[IsIPFraud]			= bb.IsIPFraud
		,	[IP]				= bb.Ip
		,	[MergeGroupId]		= bb.MergeGroupId
	FROM #tmpMatchBettypeBetteamSumStake AS r WITH(NOLOCK)
		INNER JOIN #tmpBettransAssociation bb WITH(NOLOCK) ON r.MergeGroupId = bb.MergeGroupId AND r.MatchId = bb.MatchId
					AND r.Bettype = bb.Bettype AND r.TotalLiveScore = bb.TotalLiveScore AND r.Handicap = bb.Handicap
	WHERE r.TotalStakeH > @MaxStakeToBeChecked AND r.TotalStakeA > @MaxStakeToBeChecked
	ORder by MatchId, TotalLiveScore, Bettype, Handicap, transid

	--=======UPDATE: ISFraud by Match, totallivesoscore, bettype, Handicap
	INSERT INTO #tmpFraudIP(MatchId, TotalLiveScore, Bettype, Handicap, IP)
	SELECT t.matchid, TotalLiveScore, Bettype, Handicap, IP 
	FROM #tmpFraudTickets t
	GROUP BY t.matchid, TotalLiveScore, Bettype, Handicap, IP
	HAVING	Count(Distinct GroupId) > 1

	UPDATE	ft
	SET		ft.IsIPFraud = 1
	FROM	#tmpFraudTickets ft WITH(ROWLOCK)
			INNER JOIN #tmpFraudIP fi WITH(NOLOCK) ON ft.MatchId = fi.MatchId AND ft.TotalLiveScore = ft.TotalLiveScore AND ft.Bettype = fi.Bettype 
					AND ft.Handicap = fi.Handicap AND ft.IP = fi.IP
	
	-- Check the results and store/return data
	IF EXISTS (SELECT TOP(1) 1 FROM #tmpFraudTickets WITH (NOLOCK))
	BEGIN		
		DELETE si
		FROM dbo.ArbitrageTicketDetails si WITH(ROWLOCK)
			INNER JOIN #tmpFraudTickets ft WITH(NOLOCK) ON si.MatchId = ft.MatchId AND si.BetType = ft.BetType
				 AND si.Handicap = ft.Handicap AND si.TotalScores = ft.TotalLiveScore AND si.CustId = ft.CustId;

		-- Insert new records for table ArbitrageTicketDetails
		INSERT INTO dbo.ArbitrageTicketDetails(CustId, MatchId, GroupId, BetType, Handicap, TotalScores, TransIds, TicketCounter, IsIPFraud)
		SELECT	[CustId]			= ft.CustId
			,	[MatchId]			= ft.MatchId
			,	[GroupId]			= ft.MergeGroupId
			,	[Bettype]			= ft.Bettype
			,	[Handicap]			= ft.Handicap
			,	[TotalScores]		= ft.TotalLiveScore
			,	[TransIds]			= STUFF((SELECT ',', CONVERT(VARCHAR(15), t1.TransId) 
											FROM #tmpFraudTickets AS t1 WITH(NOLOCK)
											WHERE t1.MatchId = ft.MatchId AND t1.BetType = ft.BetType AND t1.Handicap = ft.Handicap 
												AND t1.TotalLiveScore = ft.TotalLiveScore AND t1.CustId = ft.CustId AND t1.MergeGroupId = ft.MergeGroupId  AND t1.IsIPFraud = ft.IsIPFraud												
											FOR XML PATH('')), 1, 1, '')
			,	[TicketCounter]		= (SELECT COUNT(t2.TransId) 
										FROM #tmpFraudTickets t2 WITH (NOLOCK)
										WHERE t2.MatchId = ft.MatchId AND t2.BetType = ft.BetType AND t2.Handicap = ft.Handicap 
												AND t2.TotalLiveScore = ft.TotalLiveScore AND t2.CustId = ft.CustId AND t2.MergeGroupId = ft.MergeGroupId  AND t2.IsIPFraud = ft.IsIPFraud) 
			,	[IsIPFraud]			= ft.IsIPFraud
		FROM #tmpFraudTickets ft WITH (NOLOCK)
		GROUP BY ft.CustId, ft.MatchId, ft.MergeGroupId, ft.Bettype, ft.Handicap, ft.TotalLiveScore,ft.IsIPFraud;		

		-- Update table SuspiciousInfo if existed
		UPDATE si WITH (ROWLOCK, UPDLOCK)
		SET		si.SuspiciousDate	=	GETDATE()
			,	si.TransIds			=	STUFF((SELECT DISTINCT ',', CONVERT(VARCHAR(MAX), t1.TransIds) 
										FROM dbo.ArbitrageTicketDetails t1 WITH (NOLOCK)
										WHERE t1.MatchId = ft.MatchId AND t1.CustId = ft.CustId
										FOR XML PATH('')), 1, 1, '')
			,	si.TicketCounter	=	(SELECT SUM(t2.TicketCounter) 
										FROM dbo.ArbitrageTicketDetails t2 WITH (NOLOCK)
										WHERE t2.MatchId = ft.MatchId AND t2.CustId = ft.CustId)
			,	si.IsVerified		=	0
			,	si.ViolationCounter =	(SELECT COUNT(1)
										FROM (SELECT DISTINCT t2.Bettype, t2.TotalScores, t2.Handicap
										FROM dbo.ArbitrageTicketDetails AS t2 WITH (NOLOCK)
										WHERE t2.MatchId = ft.MatchId AND t2.CustId = ft.CustId) AS A)
		FROM dbo.ArbitrageSuspiciousInfo si
				INNER JOIN #tmpFraudTickets ft WITH (NOLOCK)
					ON si.MatchId = ft.MatchId AND si.CustId = ft.CustId AND si.FraudCaseId = @CurrentFraudCaseId;	
							
		-- Insert new records for table SuspiciousInfo
		INSERT INTO dbo.ArbitrageSuspiciousInfo(CustId, MatchId, EventDate, FraudCaseId, SuspiciousDate, TransIds, ViolatiONCounter, TicketCounter)
		SELECT	[CustId]			= ft.CustId
			,	[MatchId]			= ft.MatchId
			,	[EventDate]			= ft.EventDate
			,	[FraudCaseId]		= @CurrentFraudCaseId
			,	[SuspiciousDate]	= GETDATE()
			,	[TransIds]			= STUFF((SELECT DISTINCT ',', CONVERT(VARCHAR(MAX), t1.TransIds) 
										FROM dbo.ArbitrageTicketDetails t1 WITH (NOLOCK)
										WHERE t1.MatchId = ft.MatchId AND t1.CustId = ft.CustId
										FOR XML PATH('')), 1, 1, '')
			,	ViolationCounter	= (SELECT COUNT(1)
										FROM (SELECT DISTINCT t2.Bettype, t2.TotalScores, t2.Handicap
										FROM dbo.ArbitrageTicketDetails AS t2 WITH (NOLOCK)
										WHERE t2.MatchId = ft.MatchId AND t2.CustId = ft.CustId) AS A)
			,	[TicketCounter]		= (SELECT SUM(t3.TicketCounter) 
										FROM dbo.ArbitrageTicketDetails t3 WITH (NOLOCK)
										WHERE t3.MatchId = ft.MatchId AND t3.CustId = ft.CustId)
		FROM #tmpFraudTickets ft WITH (NOLOCK)
				LEFT JOIN dbo.ArbitrageSuspiciousInfo si WITH (NOLOCK) 
					ON si.MatchId = ft.MatchId AND si.CustId = ft.CustId  AND si.FraudCaseId = @CurrentFraudCaseId
		WHERE si.MatchID IS NULL
		GROUP BY ft.CustId, ft.MatchId, ft.EventDate;
			
		-- Update table SuspiciousCustomer if existed
		UPDATE sc WITH (ROWLOCK, UPDLOCK)
		SET sc.IsVerified = 0, sc.Username = ct.Username
		FROM dbo.SuspiciousCustomer sc 
				, (	SELECT DISTINCT Custid, Username
					FROM #tmpFraudTickets ft WITH (NOLOCK)) AS ct
		WHERE sc.Custid = ct.CustId

		-- Insert new records for table SuspiciousCustomer
		INSERT INTO dbo.SuspiciousCustomer(Custid, IsDelinquent, IsHedge, IsDirty, IsVerified, Username, CurrencyId, CurrencyName)
		SELECT DISTINCT ft.CustId, 0, 0, 0, 0, ft.Username, ft.CurrencyId,ft.CurrencyName
		FROM #tmpFraudTickets ft WITH (NOLOCK)
		WHERE ft.CustId NOT IN (
				SELECT sc.CustId 
				FROM dbo.SuspiciousCustomer sc WITH (NOLOCK)
			);
	END;	
	
	-- Update table ScannedMatch if existed
	UPDATE	sm WITH (ROWLOCK, UPDLOCK)
	SET		sm.ScannedDate				= GETDATE()
		,	sm.SuspiciousTicketsCount	= ISNULL((SELECT SUM(t1.TicketCounter)
											FROM dbo.ArbitrageTicketDetails t1 WITH (NOLOCK)
											WHERE t1.MatchId = m.MatchId 
											GROUP BY t1.MatchId ),0)
	FROM dbo.ScannedMatch sm
		INNER JOIN #tmpMatches m WITH (NOLOCK) ON sm.MatchId = m.MatchId AND sm.FraudCaseId = @CurrentFraudCaseId;
	
	-- Insert new records for table ScannedMatch
	INSERT INTO dbo.ScannedMatch (Matchid, FraudCaseId, ScannedDate, SuspiciousTicketsCount)  
	SELECT	DISTINCT [MatchId]			= m.matchid 
		,	[FraudCaseId]				= @CurrentFraudCaseId
		,	[ScannedDate]				= GETDATE() 
		,	[SuspiciousTicketsCount]	= ISNULL((SELECT SUM(t1.TicketCounter)
											FROM dbo.ArbitrageTicketDetails t1 WITH (NOLOCK)
											WHERE t1.MatchId = m.MatchId 
											GROUP BY t1.MatchId ),0)
	FROM #tmpMatches m
		LEFT JOIN dbo.ScannedMatch sm WITH (NOLOCK)ON m.matchid = sm.matchid AND sm.FraudCaseId = @CurrentFraudCaseId
	WHERE sm.Matchid IS NULL 
		AND sm.FraudCaseId IS NULL; 
		
	--Remove the XML documents
	EXEC sp_xml_removedocument @idocMatch;
	EXEC sp_xml_removedocument @idocAssociation;
END;

/*<info serverAlias="DBFS-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/