
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/****** Object:  Stored Procedure dbo.FS_Tbl_SuspiciousCustomer_Update    Script Date: 5/15/2018 4:21:24 PM ******/
ALTER PROCEDURE [dbo].[FS_Tbl_SuspiciousCustomer_Update]
		@UserId				INT
	,	@Custid				INT
	,	@IsDelinquent		BIT = NULL
	,	@IsHedge			BIT = NULL
	,	@IsDirty			BIT = NULL
	,	@IsVerified			BIT = NULL
	,	@ErrCode			INT = 0 OUTPUT
AS
/*
	Created: 20180504@CaseyHuynh
	Task : Update Suspicous Customer Details_Classification
	DB: bodb_FraudSensor
	Original:

	Revisions:
	Param's Explanation:    
*/
BEGIN
	SET NOCOUNT ON;
	
    DECLARE @LogTypeId INT = 12;
	DECLARE @LogData varchar(500)
			, @SPName varchar(100);
	
	DECLARE @UpdateTime DATETIME = GETDATE();
	
    BEGIN TRY
		UPDATE [dbo].[SuspiciousCustomer] WITH(ROWLOCK, UPDLOCK)
		SET [IsDelinquent] = ISNULL(@IsDelinquent,IsDelinquent)
		  ,[IsHedge] = ISNULL(@IsHedge,IsHedge)
		  ,[IsDirty] = ISNULL(@IsDirty,IsDirty)
		  ,[IsVerified] = ISNULL(@IsVerified,IsVerified)
		WHERE Custid = @Custid
	END TRY
	BEGIN CATCH
		SELECT @ErrCode = ERROR_NUMBER();
	END CATCH;
	
	--================ Write Customer Log ================ 
	If (COALESCE(@IsDelinquent,@IsHedge,@IsDirty) IS NOT NULL)
	BEGIN
		DECLARE @LogDes NVARCHAR(500);
				
		
		SELECT @LogDes='Update Classification: ';
		SELECT @LogDes = CASE WHEN @IsDelinquent = 1 THEN @LogDes + 'Marked Delinquent' 
								WHEN @IsDelinquent = 0 THEN @LogDes + 'Unmarked Delinquent'
						 ELSE @LogDes END;
		SELECT @LogDes = CASE WHEN @IsHedge = 1 THEN @LogDes + 'Marked Hedge' 
								WHEN @IsHedge = 0 THEN @LogDes + 'Unmarked Hedge'
						 ELSE @LogDes END;
		SELECT @LogDes = CASE WHEN @IsDirty = 1 THEN @LogDes + 'Marked Dirty' 
								WHEN @IsDirty = 0 THEN @LogDes + 'Unmarked Dirty'
						 ELSE @LogDes END;
						 
		EXECUTE dbo.FS_Tbl_CustomerLog_Insert @CustId = @Custid, @UserId = @UserId, @CustLogType = 2 , @LogDate = @UpdateTime, @CaseId = NULL, @LogDesc=@LogDes;
	END;
	
	--================ Write Log ================ 
	IF (@ErrCode = 0) 
	BEGIN
		SELECT @LogData =	'UserId = ' + DBO.SPU_ToString(@UserId)
							+ 'CustId = ' + DBO.SPU_ToString(@CustId) 
							+ ', IsDelinquent = ' + DBO.SPU_ToString(@IsDelinquent) 
							+ ', IsDirty = ' + DBO.SPU_ToString(@IsDirty) 
							+ ', IsVerified = ' + DBO.SPU_ToString(@IsVerified) 
							+ ', CreatorId = ' + DBO.SPU_ToString(@UserId);

		SELECT @SPName = OBJECT_NAME(@@ProcID);

		EXEC [dbo].[FS_UserLog_Insert]  
				@LogTypeId = @LogTypeId   
			,	@UserId = @UserId  
			,	@SPName = @SPName
			,	@Details = @LogData;
	END;

END;

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


GO 
 GRANT EXECUTE ON [dbo].[FS_Tbl_SuspiciousCustomer_Update] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_Tbl_SuspiciousCustomer_Update] TO [bodbSPUNet]
GO 
