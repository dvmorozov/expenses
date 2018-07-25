
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 14/07/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/3db6842f-dd5c-49e0-8536-e637ea009cd5
-- =============================================

CREATE PROCEDURE [expenses].TodayExpensesSumsByUser @Today DATETIME, @DataOwner UNIQUEIDENTIFIER 
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
	
	SELECT Name, SUM(Cost) AS Cost, CategoryName, ExpenseEncryptedName, CategoryEncryptedName, Currency
	FROM @T
	GROUP BY CategoryEncryptedName, CategoryName, ExpenseEncryptedName, Name, Currency
	ORDER BY Currency, Cost DESC
