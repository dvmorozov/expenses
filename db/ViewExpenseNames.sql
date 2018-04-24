
DROP VIEW [expenses].[ExpenseNames]
GO

CREATE VIEW [expenses].[ExpenseNames]
AS
SELECT LTRIM(RTRIM(Name)) AS Name, MIN(ID) AS ID, DataOwner
FROM expenses.Expenses
GROUP BY Name, DataOwner
GO
