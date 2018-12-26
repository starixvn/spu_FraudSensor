SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER PROCEDURE [dbo].[FS_Tbl_SuspiciousCustomerNotes_Update]
		@CustId			INT
	,	@Notes			NVARCHAR(2000) = NULL
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
	
	UPDATE	dbo.SuspiciousCustomer WITH(ROWLOCK)
	SET		Notes = @Notes
	WHERE	CustId = @CustId;
END;

GO
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/

