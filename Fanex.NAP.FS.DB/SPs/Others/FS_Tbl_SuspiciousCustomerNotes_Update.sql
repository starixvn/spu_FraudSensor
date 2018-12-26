SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER PROCEDURE [dbo].[FS_Tbl_SuspiciousCustomerNotes_Update]
		@UserId			INT 
	,	@CustId			INT
	,	@Notes			NVARCHAR(2000) = NULL
 
	,	@ErrCode		INT = 0 OUTPUT  
AS
/*
	Created: 20181226@Long
	Task : Update Notes for SuspiciousCustomer table
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20181226@Long: Created [Redmine ID: #106145]

	Param's Explanation:    
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @LogData				VARCHAR(500)
		,	@SPName					VARCHAR(100)
		,	@CurrentFraudCaseId		TINYINT;

	SELECT	@CurrentFraudCaseId		= 11; -- Bets Near Goal

	BEGIN TRY  
		UPDATE	dbo.SuspiciousCustomer WITH(ROWLOCK)
		SET		Notes = @Notes
		WHERE	CustId = @CustId;

		------------ Write Log ------------     
		SELECT @LogData = 'CustId=' + DBO.SPU_ToString(@CustId) +', Notes=' + @Notes;
		SELECT @SPName = OBJECT_NAME(@@ProcID);  

		INSERT INTO dbo.CustomerLog(CustId, CreatorId, CustLogType, CaseId, LogDesc, LogDate)
		VALUES(@CustId, @UserId, 1, @CurrentFraudCaseId, @LogData, GETDATE()); -- CustLogType = 1: Note
	END TRY  
	BEGIN CATCH  
		SET @ErrCode = ERROR_NUMBER();  
	END CATCH; 
END;

GO
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/

