
CREATE VIEW [expenses].[Expenses]
AS
SELECT ID, Date, Cost, LastUpdate, Note, DataOwner, Monthly, FirstMonth, LastMonth, Currency, Rating, Importance, EncryptedName, NameChecksum, Project, Name
FROM     expenses.Operations
WHERE  (Income IS NULL) OR (Income = 0)

