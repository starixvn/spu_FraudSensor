USE [bodb_FraudSensor]
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
ALTER PROCEDURE [dbo].[FS_FraudCase_GetAll]
	@IsInternal BIT = NULL
	, @IsActive BIT	= NULL	
	, @Environment INT	= 3
AS
/*
	Created: 20180306@Aaron
	Task : Get all fraud cases
	DB: bodb_FraudSensor

	Original:

	Revisions: 
	 + 20180414@Casey: Add parameters @IsInternal, @IsActive, @IsTest
	 + 20180605@CaseyHuynh: Update @IsInternal Default Value is NULL

	Param's Explanation (filtered by):
	 + @IsInternal: NULL-All, 0-NotInternal, 1-Internal
	 + @IsAcitve: NULL-All, 0-Inactive, 1-Active
	 + @Environment: 3-Production get Istest=0 ELSE All	
*/
BEGIN
	SET NOCOUNT ON;	
    SELECT	fc.FraudCaseId
		,	fc.FraudCaseName
		,	fc.FraudCaseDesc
		,	fc.FraudCaseInterval
		,	fc.IsActive
		,	fc.IsTest
	FROM	dbo.FraudCase fc WITH (NOLOCK)
	WHERE	fc.IsInternal = ISNULL(@IsInternal,fc.IsInternal)
			AND fc.IsActive = ISNULL(@IsActive,fc.IsActive)
			AND fc.IsTest = (CASE WHEN @Environment = 3 THEN 0 ELSE fc.IsTest END)
	ORDER BY FraudCaseId;
END;
GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_GetAll] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_GetAll] TO [bodbSPUNet]
GO 
