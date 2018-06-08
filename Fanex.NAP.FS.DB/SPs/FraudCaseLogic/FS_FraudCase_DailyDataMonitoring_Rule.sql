USE [bodb_FraudSensor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_DailyDataMonitoring_Rule] 
AS
/*
	Created: 20180607@LONG
	Task : Rule for Daily Data Monitoring fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:

	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	

	DECLARE @NewCapturedDate		DATETIME
		,	@OldCapturedDate		DATETIME
		,	@CaptureDate			DATETIME
		,	@TotalChangedMatches	INT
		,	@CurrentFraudCaseId		INT;

	SELECT	@CaptureDate = GETDATE()
		,	@CurrentFraudCaseId = 4;
			
	IF	OBJECT_ID('tempdb..#tmpMatchesData') IS NOT NULL
	BEGIN;
		DROP TABLE #tmpMatchesData;
	END;

	CREATE TABLE	#tmpMatchesData (
		[MatchId]					INT NOT NULL,
		[TotalCompanyTO]			MONEY NULL,
		[TotalSuperTO]				MONEY NULL,
		[TotalMasterTO]				MONEY NULL,
		[TotalAgentTO]				MONEY NULL,
		[TotalTickets]				INT NULL,
		[TotalSettledTickets]		INT NULL,
		[TotalCancelledTickets]		INT NULL,
		[CapturedDate]				DATETIME NULL
	);

	CREATE CLUSTERED INDEX	#CIX_tmpMatchesData_MatchId ON #tmpMatchesData (
		[MatchId]					ASC
	);

	IF	OBJECT_ID('tempdb..#tmpMatchesOldData') IS NOT NULL
	BEGIN;
		DROP TABLE #tmpMatchesOldData;
	END;

	CREATE TABLE	#tmpMatchesOldData (
		[MatchId]					INT NOT NULL,
		[TotalCompanyTO]			MONEY NULL,
		[TotalSuperTO]				MONEY NULL,
		[TotalMasterTO]				MONEY NULL,
		[TotalAgentTO]				MONEY NULL,
		[TotalTickets]				INT NULL,
		[TotalSettledTickets]		INT NULL,
		[TotalCancelledTickets]		INT NULL
	);

	CREATE CLUSTERED INDEX	#CIX_tmpMatchesOldData_MatchId ON #tmpMatchesOldData (
		[MatchId]					ASC
	);

	IF	OBJECT_ID('tempdb..#tmpMatchesNewData') IS NOT NULL
	BEGIN;
		DROP TABLE #tmpMatchesNewData;
	END;

	CREATE TABLE	#tmpMatchesNewData (
		[MatchId]					INT NOT NULL,
		[TotalCompanyTO]			MONEY NULL,
		[TotalSuperTO]				MONEY NULL,
		[TotalMasterTO]				MONEY NULL,
		[TotalAgentTO]				MONEY NULL,
		[TotalTickets]				INT NULL,
		[TotalSettledTickets]		INT NULL,
		[TotalCancelledTickets]		INT NULL
	);

	CREATE CLUSTERED INDEX	#CIX_tmpMatchesNewData_MatchId ON #tmpMatchesNewData (
		[MatchId]					ASC
	);
		
	-------- START --------
	SELECT @NewCapturedDate = MAX(cd.CapturedDate)
	FROM dbo.CapturedData AS cd WITH(NOLOCK);

	SELECT @OldCapturedDate = MAX(cd.CapturedDate)
	FROM dbo.CapturedData AS cd WITH(NOLOCK)
	WHERE cd.CapturedDate <> @NewCapturedDate;

	INSERT INTO #tmpMatchesData(MatchId, TotalCompanyTO, TotalSuperTO, TotalMasterTO, TotalAgentTO, TotalTickets, TotalSettledTickets, TotalCancelledTickets, CapturedDate)
	SELECT	[MatchId]					= cd.matchid
		,	[TotalCompanyTO]			= cd.TotalCompanyTO
		,	[TotalSuperTO]				= cd.TotalSuperTO
		,	[TotalMasterTO]				= cd.TotalMasterTO
		,	[TotalAgentTO]				= cd.TotalAgentTO
		,	[TotalTickets]				= cd.TotalTickets
		,	[TotalSettledTickets]		= cd.TotalSettledTickets
		,	[TotalCancelledTickets]		= cd.TotalCancelledTickets
		,	[CapturedDate]				= cd.CapturedDate
	FROM dbo.CapturedData AS cd WITH(NOLOCK)
	WHERE cd.CapturedDate = @OldCapturedDate OR cd.CapturedDate = @NewCapturedDate;

	DELETE FROM #tmpMatchesData
	WHERE MatchId IN
        (
            SELECT MatchId
            FROM #tmpMatchesData
            GROUP BY MatchId
            HAVING COUNT(1) < 2
        );

	INSERT INTO #tmpMatchesOldData(MatchId, TotalCompanyTO, TotalSuperTO, TotalMasterTO, TotalAgentTO, TotalTickets, TotalSettledTickets, TotalCancelledTickets)
	SELECT	[MatchId]					= cd.matchid
		,	[TotalCompanyTO]			= cd.TotalCompanyTO
		,	[TotalSuperTO]				= cd.TotalSuperTO
		,	[TotalMasterTO]				= cd.TotalMasterTO
		,	[TotalAgentTO]				= cd.TotalAgentTO
		,	[TotalTickets]				= cd.TotalTickets
		,	[TotalSettledTickets]		= cd.TotalSettledTickets
		,	[TotalCancelledTickets]		= cd.TotalCancelledTickets
	FROM #tmpMatchesData AS cd WITH(NOLOCK)
	WHERE cd.CapturedDate = @OldCapturedDate;

	INSERT INTO #tmpMatchesNewData(MatchId, TotalCompanyTO, TotalSuperTO, TotalMasterTO, TotalAgentTO, TotalTickets, TotalSettledTickets, TotalCancelledTickets)
	SELECT	[MatchId]					= cd.matchid
		,	[TotalCompanyTO]			= cd.TotalCompanyTO
		,	[TotalSuperTO]				= cd.TotalSuperTO
		,	[TotalMasterTO]				= cd.TotalMasterTO
		,	[TotalAgentTO]				= cd.TotalAgentTO
		,	[TotalTickets]				= cd.TotalTickets
		,	[TotalSettledTickets]		= cd.TotalSettledTickets
		,	[TotalCancelledTickets]		= cd.TotalCancelledTickets
	FROM #tmpMatchesData AS cd WITH(NOLOCK)
	WHERE cd.CapturedDate = @NewCapturedDate;

	INSERT INTO dbo.ChangedMatch(MatchId, OldTotalCompanyTO, NewTotalCompanyTO, OldTotalSuperTO, NewTotalSuperTO, OldTotalMasterTO, NewTotalMasterTO, OldTotalAgentTO, NewTotalAgentTO, OldTotalTickets, NewTotalTickets, OldTotalSettledTickets, NewTotalSettledTickets, OldTotalCancelledTickets, NewTotalCancelledTickets, CapturedDate)
	SELECT	[MatchId]					= old.MatchId
		,	[OldTotalCompanyTO]			= old.TotalCompanyTO
		,	[NewTotalCompanyTO]			= new.TotalCompanyTO
		,	[OldTotalSuperTO]			= old.TotalSuperTO
		,	[NewTotalSuperTO]			= new.TotalSuperTO
		,	[OldTotalMasterTO]			= old.TotalMasterTO
		,	[NewTotalMasterTO]			= new.TotalMasterTO
		,	[OldTotalAgentTO]			= old.TotalAgentTO
		,	[NewTotalAgentTO]			= new.TotalAgentTO
		,	[OldTotalTickets]			= old.TotalTickets
		,	[NewTotalTickets]			= new.TotalTickets
		,	[OldTotalSettledTickets]	= old.TotalSettledTickets
		,	[NewTotalSettledTickets]	= new.TotalSettledTickets
		,	[OldTotalCancelledTickets]	= old.TotalCancelledTickets
		,	[NewTotalCancelledTickets]	= old.TotalCancelledTickets
		,	[CapturedDate]				= @CaptureDate
	FROM #tmpMatchesOldData AS old WITH(NOLOCK)
		INNER JOIN #tmpMatchesNewData AS new WITH(NOLOCK) ON old.MatchId = new.MatchId
	WHERE old.TotalCompanyTO <> new.TotalCompanyTO
		OR old.TotalMasterTO <> new.TotalMasterTO
		OR old.TotalSuperTO <> new.TotalSuperTO
		OR old.TotalAgentTO <> new.TotalAgentTO
		OR old.TotalTickets <> new.TotalTickets
		OR old.TotalSettledTickets <> new.TotalSettledTickets
		OR old.TotalCancelledTickets <> new.TotalCancelledTickets;

	-------- LOG MESSAGE --------
	SELECT @TotalChangedMatches = COUNT(DISTINCT cm.MatchId)
	FROM dbo.ChangedMatch AS cm WITH(NOLOCK) 
	WHERE cm.CapturedDate = @CaptureDate;

	IF (@TotalChangedMatches > 0)
	BEGIN
		INSERT INTO dbo.[Message](FraudCaseId, MessageContent, LogDate)
		VALUES(@CurrentFraudCaseId, 'There are ' +  CAST(@TotalChangedMatches AS VARCHAR(5)) + ' matches which having data changes on ' + CAST(@CaptureDate AS VARCHAR(20)), @CaptureDate);

		DECLARE @NewlyMessageId		INT;
		SELECT @NewlyMessageId = SCOPE_IDENTITY();

		INSERT INTO dbo.UserMessage(UserId, MessageId)
		SELECT	[UserId] = r.RecipientId
			,	[MessageId] = @NewlyMessageId
		FROM dbo.FraudNotificationRecipient AS r WITH(NOLOCK)
		WHERE r.FraudCaseId = @CurrentFraudCaseId;
	END;
END;

GO
GRANT EXECUTE ON [dbo].[FS_FraudCase_DailyDataMonitoring_Rule] TO [bodbSPUNet]
GO
GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_DailyDataMonitoring_Rule] TO [bodbSPUNet]
GO
