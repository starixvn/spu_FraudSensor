
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER PROCEDURE [dbo].[FS_Rpt_SuspiciousCustomer_Get] 
		@FromDate					DATETIME
	,	@ToDate						DATETIME
	,	@Username					NVARCHAR(50) = NULL	
	,	@FraudCaseId				TINYINT = 0
	,	@IsVerified					BIT = NULL
	,	@IsDelinquent				BIT = NULL
	,	@IsDirty					BIT = NULL
	,	@IsHedge					BIT = NULL
	,	@AssigneeId					INT = NULL
	,	@Environment				INT = 3
	,	@IsShowSpecialCurrencies	DATETIME
AS
/*
	Created: 20180320@Long
	Task : Suspicious Customers report
	DB: bodb_FraudSensor
	Original:

	Revisions:
		-	20180326@CaseyHuynh: 
			+ Remove "Having Condition on IsVerified value"
			+ Update @AssingeeId = 0 If filter By "All"
			+ Return Assignee Username Instead Of AssigneeId
		- 20181114@Long: Adding View Permission on Special Currencies [Redmine ID: #104707]

	Param's Explanation:    
			+ Add Condition for Customer Log GET: cl.LogType = 1
*/
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#tmpFraudCases') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpFraudCases;
	END

	CREATE TABLE #tmpFraudCases (
			FraudCaseId			INT
		PRIMARY KEY CLUSTERED (FraudCaseId)
	);

	IF @FraudCaseId = 0
		BEGIN
			INSERT INTO #tmpFraudCases(FraudCaseId)
			SELECT fc.FraudCaseId
			FROM dbo.FraudCase AS fc WITH(NOLOCK)
			WHERE fc.IsTest = (CASE WHEN @Environment = 3 THEN 0 ELSE fc.IsTest END)
		END;
	ELSE
		BEGIN
			INSERT INTO #tmpFraudCases(FraudCaseId)
			SELECT fc.FraudCaseId
			FROM dbo.FraudCase AS fc WITH(NOLOCK)
			WHERE fc.FraudCaseId = @FraudCaseId AND fc.IsTest = (CASE WHEN @Environment = 3 THEN 0 ELSE fc.IsTest END)
		END;

	SELECT	[CustId]			= si.custid
		,	[Username]			= c.username
		,	[HasNote]			= (CASE WHEN (
												SELECT COUNT(DISTINCT cl.CustId)
												FROM dbo.CustomerLog AS cl WITH (NOLOCK)
												WHERE cl.CustId = si.CustId AND cl.CustLogType = 1
											) > 0
										THEN 1
									ELSE 0
									END)
		,	[IsDelinquent]		= sc.IsDelinquent
		,	[IsDirty]			= sc.IsDirty
		,	[IsHedge]			= sc.IsHedge
		,	[IsVerified]		= sc.IsVerified
		,	[TotalViolation]	= SUM(si.ViolationCounter)
		,	[ViolatedCases]		= COUNT(DISTINCT si.FraudCaseId)
		,	[BetCount]			= SUM(si.TicketCounter)
		,	[Assignee]			= (SELECT TOP 1 username FROM bodb02.dbo.Customer c WITH(NOLOCK) WHERE c.custid = cma.AssigneeId)
	FROM dbo.SuspiciousInfo si WITH(NOLOCK)
		INNER JOIN dbo.SuspiciousCustomer sc WITH(NOLOCK) ON si.CustId = sc.Custid
		INNER JOIN bodb02.dbo.Customer c WITH(NOLOCK) ON si.CustId = c.CustId
		INNER JOIN #tmpFraudCases fc WITH(NOLOCK) ON fc.FraudCaseId = si.FraudCaseId
		LEFT JOIN dbo.CustomerMonitorAssignment AS cma WITH (NOLOCK) ON cma.CustId = si.CustId
	WHERE (si.EventDate BETWEEN @FromDate AND @ToDate)
		AND (@IsDelinquent IS NULL OR sc.IsDelinquent = @IsDelinquent)
		AND (@IsHedge IS NULL OR sc.IsHedge = @IsHedge)
		AND (@IsDirty IS NULL OR sc.IsDirty = @IsDirty)
		AND (@IsVerified IS NULL OR sc.IsVerified = @IsVerified)
		AND (ISNULL(cma.AssigneeId,0) = CASE WHEN @AssigneeId IS NULL THEN ISNULL(cma.AssigneeId,0)
									WHEN @AssigneeId = - 1 THEN 0
									WHEN @AssigneeId >=0 THEN @AssigneeId
									END
			)
		AND (@Username IS NULL OR c.username = @Username)		
		AND (@IsShowSpecialCurrencies = 1 OR si.CurrencyId NOT IN (SELECT CurrencyId FROM dbo.SpecialCurrency WITH(NOLOCK)))
	GROUP BY si.custid
		,	c.username
		,	sc.IsDelinquent
		,	sc.IsDirty
		,	sc.IsHedge
		,	sc.IsVerified
		,	cma.AssigneeId
	ORDER BY sc.IsDirty		DESC
		,	ViolatedCases	DESC
		,	TotalViolation	DESC
		,	BetCount		DESC
		,	c.username		DESC
		;
END;

GO
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/
