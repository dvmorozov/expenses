
DROP PROCEDURE TodayExpenses
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE TodayExpenses @Today DATETIME AS
	SELECT Date, Name, Cost, Note 
	FROM Expenses 
	WHERE CAST(Date AS Date) = @Today
	ORDER BY ID DESC
GO

DROP PROCEDURE TodayExpensesByUser3
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 12/07/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
-- =============================================

CREATE PROCEDURE TodayExpensesByUser3 @Today DATETIME, @DataOwner UNIQUEIDENTIFIER 
AS
	SELECT 
		e.Date, LTRIM(RTRIM(e.Name)) AS Name, e.Cost, LTRIM(RTRIM(e.Note)) AS Note, e.ID, LTRIM(RTRIM(c.Name)) AS CategoryName,
		e.EncryptedName AS ExpenseEncryptedName, c.EncryptedName AS CategoryEncryptedName, e.Currency
	FROM Expenses e
	INNER JOIN ExpensesCategories ec
	ON ec.ExpenseID = e.ID
	INNER JOIN Categories c
	ON ec.CategoryID = c.ID
	WHERE e.DataOwner = @DataOwner 
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		AND (
				(
					(e.Monthly IS NULL OR e.Monthly = 0) AND
					DATEPART(YEAR, DATE) = DATEPART(YEAR, @Today) AND 
					DATEPART(MONTH, DATE) = DATEPART(MONTH, @Today) AND
					DATEPART(DAY, DATE) = DATEPART(DAY, @Today)
				)
			OR
				(
					(e.Monthly IS NOT NULL AND e.Monthly = 1) AND
					DATEPART(DAY, Date) = DATEPART(DAY, @Today) AND
					(@Today >= e.FirstMonth) AND (@Today <= EOMONTH(e.LastMonth) OR LastMonth IS NULL)
				)
			)
	--	The last expenses first.
	ORDER BY ID DESC
GO


DROP PROCEDURE TodayExpensesSumsByUser 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 14/07/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/3db6842f-dd5c-49e0-8536-e637ea009cd5
-- =============================================

CREATE PROCEDURE TodayExpensesSumsByUser @Today DATETIME, @DataOwner UNIQUEIDENTIFIER 
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

	INSERT INTO @T EXEC TodayExpensesByUser3 @Today, @DataOwner 
	
	SELECT Name, SUM(Cost) AS Cost, CategoryName, ExpenseEncryptedName, CategoryEncryptedName, Currency
	FROM @T
	GROUP BY CategoryEncryptedName, CategoryName, ExpenseEncryptedName, Name, Currency
	ORDER BY Currency, Cost DESC
GO

DROP PROCEDURE TodayExpensesByUser2
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 20/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================

CREATE PROCEDURE TodayExpensesByUser2 @Today DATETIME, @DataOwner UNIQUEIDENTIFIER 
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

	INSERT INTO @T EXEC TodayExpensesByUser3 @Today, @DataOwner 
	
	SELECT Date, Name, Cost, Note, ID, CategoryName, ExpenseEncryptedName, CategoryEncryptedName
	FROM @T
GO

DROP PROCEDURE TodayExpensesByUser
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE TodayExpensesByUser @Today DATETIME, @DataOwner UNIQUEIDENTIFIER 
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

	INSERT INTO @T EXEC TodayExpensesByUser3 @Today, @DataOwner 
	
	SELECT Date, Name, Cost, Note, ID, CategoryName
	FROM @T
GO

