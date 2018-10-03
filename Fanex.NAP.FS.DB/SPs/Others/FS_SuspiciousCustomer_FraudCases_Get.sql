SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [dbo].[FS_SuspiciousCustomer_FraudCases_Get]
		@CustId	INT
	,	@Environment		INT = 3
AS
/*
	Created: 20180504@CaseyHuynh
	Task : Get Suspicous Customer Details_FraudCasesVerify
	DB: bodb_FraudSensor
	Original:

	Revisions:
			20181002@JohnNgo: Check @Environment for Fraudcase
	
	Param's Explanation:    
			+ @Environment: 3-Production get Istest=0 ELSE All (Clone from [FS_FraudCase_GetAll])  
*/

BEGIN
	SET NOCOUNT ON;

	SELECT	[FraudCaseId]		=	fs.FraudCaseId
		,	[FraudCaseName]		=	fs.FraudCaseName
		,	[Violations]		=	fs.Vilolations
		,	[Verified]			=	fs.Verified
		,	[Unverified]		=	fs.Vilolations - fs.Verified
		,	[LastVerifiedDate]	=	fs.LastVerifiedDate
		,	[LastVerifiedBy]	=	(SELECT TOP(1) ct.username 
									FROM bodb02.dbo.Customer AS ct WITH(NOLOCK) 
										INNER JOIN dbo.SuspiciousInfo AS su WITH(NOLOCK) ON su.VerifiedBy = ct.custid
									WHERE su.VerifiedDate=fs.LastVerifiedDate
											AND su.FraudCaseId = fs.FraudCaseId)
	FROM (
			SELECT	[FraudCaseId]		=	si.FraudCaseId
				,	[FraudCaseName]		=	(SELECT TOP(1) fc.FraudCaseName FROM dbo.FraudCase AS fc WITH(NOLOCK) WHERE fc.FraudCaseId = si.FraudCaseId)
				,	[Vilolations]		=	SUM(si.ViolationCounter)
				,	[Verified]			=	SUM(CASE WHEN si.IsVerified = '1' THEN ViolationCounter ELSE 0 END) 
				,	[LastVerifiedDate]	=	MAX(si.VerifiedDate)
			FROM dbo.SuspiciousInfo AS si WITH(NOLOCK)
			WHERE CustId = @CustId
			GROUP BY si.FraudCaseId) AS fs
	INNER JOIN dbo.FraudCase c WITH (NOLOCK) ON c.FraudCaseId = fs.FraudCaseId
	WHERE c.IsTest = (CASE WHEN @Environment = 3 THEN 0 ELSE c.IsTest END)
	ORDER BY fs.FraudCaseName Asc;

END;

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GO 
 GRANT EXECUTE ON [dbo].[FS_SuspiciousCustomer_FraudCases_Get] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_SuspiciousCustomer_FraudCases_Get] TO [bodbSPUNet]
GO 

GO
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/