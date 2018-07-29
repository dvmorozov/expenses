
CREATE PROCEDURE [expenses].TodayExpenses @Today DATETIME AS
	SELECT Date, Name, Cost, Note 
	FROM [expenses].Expenses 
	WHERE CAST(Date AS Date) = @Today
	ORDER BY ID DESC
