
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER PROCEDURE [dbo].[FS_Tbl_SuspiciousCustomer_Get]
		@Custid						INT
	,	@IsDelinquent				BIT = NULL
	,	@IsHedge					BIT = NULL
	,	@IsDirty					BIT = NULL
	,	@IsShowSpecialCurrencies	BIT
AS
/*
	Created: 20180504@CaseyHuynh
	Task : Update Suspicous Customer Details_Classification
	DB: bodb_FraudSensor
	Original:

	Revisions:
		- 20181114@Long: Adding View Permission on Special Currencies [Redmine ID: #104707]

	Param's Explanation:    
*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT si.Custid, si.IsDelinquent, si.IsDirty, si.IsHedge, si.IsVerified
	FROM dbo.SuspiciousCustomer si WITH(NOLOCK)
		INNER JOIN bodb02.dbo.Customer c WITH(NOLOCK) ON si.Custid = c.custid
	WHERE si.Custid				=	ISNULL(@Custid,si.Custid)
		  AND si.[IsDelinquent]	=	ISNULL(@IsDelinquent,si.IsDelinquent)
		  AND si.[IsHedge]		=	ISNULL(@IsHedge,si.IsHedge)
		  AND si.[IsDirty]		=	ISNULL(@IsDirty,si.IsDirty)
		  AND (@IsShowSpecialCurrencies = 1 OR c.currency NOT IN (SELECT CurrencyId FROM dbo.SpecialCurrency WITH(NOLOCK)));
END;

GO
/*<info serverAlias="DBSPU-bodb_FraudSensor" executers="bodbSPUNet" isFunction="0" isNested="0"></info>*/

