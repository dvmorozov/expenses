
DROP VIEW ExpenseNames
GO

CREATE VIEW ExpenseNames
AS
SELECT LTRIM(RTRIM(Name)) AS Name, MIN(ID) AS ID, DataOwner
FROM expenses.Expenses
GROUP BY Name, DataOwner
GO
