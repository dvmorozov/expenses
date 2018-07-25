
CREATE PROCEDURE [expenses].TodayExpensesByUser @Today DATETIME, @DataOwner UNIQUEIDENTIFIER 
AS
	DECLARE @T TABLE (
		Date DATETIME NOT NULL, 
		Name NCHAR(50) NOT NULL,
		Cost FLOAT NULL,
		Note NCHAR(200) NULL,
		ID INT NOT NULL,
		CategoryName CHAR(100) NOT NULL,
		ExpenseEncryptedName NVARCHAR(MAX) NULL,
		CategoryEncryptedName NVARCHAR(MAX) NULL,
		Currency NCHAR(5) NULL
		)

	INSERT INTO @T EXEC [expenses].TodayExpensesByUser3 @Today, @DataOwner 
	
	SELECT Date, Name, Cost, Note, ID, CategoryName
	FROM @T
