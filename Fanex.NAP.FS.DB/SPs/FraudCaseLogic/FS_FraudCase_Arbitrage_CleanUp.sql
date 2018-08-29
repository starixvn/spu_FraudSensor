USE [bodb_FraudSensor]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_Arbitrage_CleanUp] 
	@MatchIds		VARCHAR(MAX) = NULL
AS
/*
	Created: 20180816@Long
	Task : Clean up cached data for Arbitrage fraud case
	DB: bodb_FraudSensor
	Original:

	Revisions:

	Param's Explanation (filtered by):
*/
BEGIN
	SET NOCOUNT ON;	
	
	IF OBJECT_ID('tempdb..#tmpMatches') IS NOT NULL
	BEGIN
		DROP TABLE	#tmpMatches;
	END

	CREATE TABLE #tmpMatches
	(
		MatchId			INT PRIMARY KEY
	);

	INSERT INTO #tmpMatches(MatchId)
	SELECT val FROM dbo.SPU_StrSplit(@MatchIds, ',');	

	SELECT m.MatchId
	FROM #tmpMatches AS m WITH(NOLOCK)
	EXCEPT
	SELECT DISTINCT ms.MatchId
	FROM [dbo].[ArbitrageMatchScoreHDP] AS ms WITH(NOLOCK)
	WHERE ms.HasScanned = 0;

	DELETE FROM [dbo].[ArbitrageMatchScoreHDP]
	WHERE MatchId IN	(
							SELECT m.MatchId
							FROM #tmpMatches AS m WITH(NOLOCK)
							EXCEPT
							SELECT DISTINCT ms.MatchId
							FROM [dbo].[ArbitrageMatchScoreHDP] AS ms WITH(NOLOCK)
							WHERE ms.HasScanned = 0
						);	
END;

GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_Arbitrage_PreRule] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_Arbitrage_PreRule] TO [bodbSPUNet]
GO 
