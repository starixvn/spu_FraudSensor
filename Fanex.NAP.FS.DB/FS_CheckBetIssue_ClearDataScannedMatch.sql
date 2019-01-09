/*
	Created: 20190109@CaseyHuynh
	Task : CLEAR Table CheckBetScannedMatch. Only Keep data in 14 days
	DB: bodb_FraudSensor
	Original:

	Revisions:

	Param's Explanation (filtered by):
*/
DECLARE @deleteDate DATE;

IF	OBJECT_ID('tempdb..#temp_ScannedDate') IS NOT NULL
BEGIN
	DROP TABLE	#temp_ScannedDate;
END; 

CREATE TABLE #temp_ScannedDate
(
	ScannedDate DATE
);


INSERT INTO #temp_ScannedDate
SELECT DISTINCT c.ScannedDate
FROM CheckBetScannedMatch c WITH(NOLOCK)
WHERE DATEDIFF(DD,c.ScannedDate,GETDATE()) >=15;

SELECT max(scannedDate) from #temp_ScannedDate

WHILE EXISTS(SELECT TOP(1) 1 FROM #temp_ScannedDate)
BEGIN
	SELECT TOP(1) @deleteDate = s.ScannedDate FROM #temp_ScannedDate s WITH(NOLOCK);

	DELETE CheckBetScannedMatch
	WHERE ScannedDate = @deleteDate;

	DELETE	#temp_ScannedDate
	WHERE	ScannedDate = @deleteDate;
END

