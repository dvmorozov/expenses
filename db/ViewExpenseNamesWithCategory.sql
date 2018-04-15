
DROP VIEW ExpenseNamesWithCategory
GO

CREATE VIEW ExpenseNamesWithCategory
AS
SELECT expenses.Expenses.ID, expenses.Expenses.Name, expenses.Expenses.DataOwner, expenses.ExpensesCategories.CategoryID, expenses.Expenses.EncryptedName
FROM expenses.Expenses INNER JOIN expenses.ExpensesCategories ON expenses.Expenses.ID = expenses.ExpensesCategories.ExpenseID
GO
