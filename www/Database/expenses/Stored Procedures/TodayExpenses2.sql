
CREATE PROCEDURE [expenses].TodayExpenses2 @Day INT, @Month INT, @Year INT
AS
	DECLARE @T DATETIME
	SET @T = CAST(CAST(@Year AS VARCHAR) + '-' + CAST(@Month AS VARCHAR) + '-' + CAST(@Day AS VARCHAR) AS DATETIME)
	EXEC TodayExpenses @T