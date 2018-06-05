USE bodb_FraudSensor
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE PROCEDURE dbo.FS_Tbl_OfficeIP_Insert
	@UserId		INT
	, @IPRange		VARCHAR(11)
	, @FromNumber	SMALLINT
	, @ToNumber		SMALLINT
	, @ErrCode		INT	= 0		OUTPUT
AS
/*
	Created: 20180605@CaseyHuynh
	Task : Insert IP To OfficeIP table
	DB: bodb_FraudSensor

	Original:

	Revisions: 


	Param's Explanation (filtered by):
	 + @IPRange: a first three part of IP Address
	 + @FromNumber: NULL-All, 0-Inactive, 1-Active
	 + @ToNumber: 3-Production get Istest=0 ELSE All	
*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @LogTypeId		INT	=	12;
	DECLARE @LogData		VARCHAR(500)
			, @SPName		VARCHAR(100);
	
	DECLARE @InsertTime DATETIME;
	
	BEGIN TRY
	
		SELECT @InsertTime = GETDATE();
					
		INSERT INTO dbo.OfficeIP(IPRange, FromNumber, ToNumber)
		VALUES(@IPRange, @FromNumber, @ToNumber);
		
	END TRY
	BEGIN CATCH
		SELECT @ErrCode	=	ERROR_NUMBER();
	END CATCH;
	
	--================ Write Log ================ 
	IF (@ErrCode	=	0) 
	BEGIN
		SELECT @LogData =	'@UserId=	'			+	DBO.SPU_ToString(@UserId)  
							+ ', IPRange	=	' 	+	DBO.SPU_ToString(@IPRange) 
							+ ', FromNumber	=	' 		+	DBO.SPU_ToString(@FromNumber)
							+ ', ToNumber	=	' 	+	DBO.SPU_ToString(@ToNumber)
							+ ', LogDate	=	' 	+	DBO.SPU_ToString(@InsertTime);

		SELECT @SPName	=	OBJECT_NAME(@@ProcID);

		EXEC [dbo].[FS_UserLog_Insert]  
				@LogTypeId	=	@LogTypeId   
			,	@UserId		=	@UserId  
			,	@SPName		=	@SPName
			,	@Details	=	@LogData;
	END;
END;
GO 
 GRANT EXECUTE ON dbo.FS_Tbl_OfficeIP_Insert TO bodbSPUNet
GO 
 GRANT VIEW DEFINITION ON dbo.FS_Tbl_OfficeIP_Insert TO bodbSPUNet
GO 