
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 20/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================

CREATE PROCEDURE [expenses].TodayExpensesByUser2 @Today DATETIME, @DataOwner UNIQUEIDENTIFIER 
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
	
	SELECT Date, Name, Cost, Note, ID, CategoryName, ExpenseEncryptedName, CategoryEncryptedName
	FROM @T
