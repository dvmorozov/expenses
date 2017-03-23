
DROP VIEW ExpenseNamesWithCategory
GO

CREATE VIEW ExpenseNamesWithCategory
AS
SELECT        dbo.Expenses.ID, dbo.Expenses.Name, dbo.Expenses.DataOwner, dbo.ExpensesCategories.CategoryID, dbo.Expenses.EncryptedName
FROM            dbo.Expenses INNER JOIN
                         dbo.ExpensesCategories ON dbo.Expenses.ID = dbo.ExpensesCategories.ExpenseID