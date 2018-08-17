USE [bodb_FraudSensor]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[FS_FraudCase_Arbitrage_PreRule] 
	@MatchIds		VARCHAR(MAX) = NULL
AS
/*
	Created: 20180816@Long
	Task : Pre-Rule for Arbitrage fraud case - Get list of CustID
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

	SELECT DISTINCT [MatchId]	= b.matchid
		,	[CustId]	= b.custid
	FROM bodb02.dbo.bettrans AS b WITH (NOLOCK)
		INNER JOIN #tmpMatches AS m WITH(NOLOCK) ON b.matchid = m.MatchId
	WHERE b.bettype IN (1, 3, 7, 8)
		AND b.Currency NOT IN (20, 27, 28);
END;

GO 
 GRANT EXECUTE ON [dbo].[FS_FraudCase_Arbitrage_PreRule] TO [bodbSPUNet]
GO 
 GRANT VIEW DEFINITION ON [dbo].[FS_FraudCase_Arbitrage_PreRule] TO [bodbSPUNet]
GO 
