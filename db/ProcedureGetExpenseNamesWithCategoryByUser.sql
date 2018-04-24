DROP PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 19/01/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/42b4d734-28a0-48b6-9403-148faa8409a2
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser 
	@CategoryID INT,
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @T TABLE (
		Name NCHAR(50) NOT NULL, 
		Id INT NOT NULL,
		EncryptedName NVARCHAR(MAX) NULL
		)

	INSERT INTO @T EXEC [expenses].GetExpenseNamesWithCategoryByUser2 @CategoryID, @DataOwner 
	
	SELECT Name, Id
	FROM @T
END
GO

DROP PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser2
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 19/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/d8e9d2dc-b1df-47af-882e-f84727e5c435
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser2 
	@CategoryID INT,
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @T TABLE (
		Name NCHAR(50) NOT NULL, 
		Id INT NOT NULL,
		EncryptedName NVARCHAR(MAX) NULL,
		Count INT NOT NULL
		)

	INSERT INTO @T EXEC [expenses].GetExpenseNamesWithCategoryByUser3 @CategoryID, @DataOwner 
	
	SELECT Name, Id, EncryptedName
	FROM @T
END
GO


DROP PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser3
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 30/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/8cc36717-9904-4f5c-bb2b-fffb8cfb37d1
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser3 
	@CategoryID INT,
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--	https://www.evernote.com/shard/s132/nl/14501366/adbb4c02-3975-460d-88f1-8a65312ca83f
	--	The last expense is selected in each group.
	SELECT e.Name, e.EncryptedName, c.Id, c.Count
	FROM [expenses].Expenses e
	JOIN 
	(
		--	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
		SELECT MAX(e.ID) AS Id, COUNT(*) AS Count
		FROM [expenses].Expenses e
		JOIN [expenses].ExpensesCategories ec
		ON e.ID = ec.ExpenseID
		WHERE CategoryID = @CategoryID AND DataOwner = @DataOwner
		GROUP BY NameChecksum
	) c
	ON c.Id = e.ID
	ORDER BY c.Count DESC
END
GO

DROP PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser4
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 05/11/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser4
	@CategoryID INT,
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--	https://www.evernote.com/shard/s132/nl/14501366/adbb4c02-3975-460d-88f1-8a65312ca83f
	--	The last expense is selected in each group.
	SELECT e.Name, e.EncryptedName, c.Id
	FROM [expenses].Expenses e
	JOIN 
	(
		--	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
		SELECT MAX(e.ID) AS Id
		FROM [expenses].Expenses e
		JOIN [expenses].ExpensesCategories ec
		ON e.ID = ec.ExpenseID
		WHERE CategoryID = @CategoryID AND DataOwner = @DataOwner
		GROUP BY NameChecksum
	) c
	ON c.Id = e.ID
END
GO

DROP PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser5
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 09/12/2015
-- Description:	https://action.mindjet.com/task/14479694
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser5
	@CategoryID INT,
	@DataOwner UNIQUEIDENTIFIER,
	@Year INT, @Month INT, @Day INT,
	@ShortList BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--	Date to start from.
	DECLARE @DateFrom DATETIME
	--	Selects for last half a year.
	SET @DateFrom = DATEADD(month, -6, GETDATE());

	--	https://www.evernote.com/shard/s132/nl/14501366/adbb4c02-3975-460d-88f1-8a65312ca83f
	--	The last expense is selected in each group.
	SELECT e.Name, e.EncryptedName, c.Id
	FROM [expenses].Expenses e
	JOIN 
	(
		--	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
		SELECT MAX(e.ID) AS Id
		FROM [expenses].Expenses e
		JOIN [expenses].ExpensesCategories ec
		ON e.ID = ec.ExpenseID
		WHERE CategoryID = @CategoryID AND DataOwner = @DataOwner
			AND
			(
				@ShortList = 0 OR 
				(Date > @DateFrom OR (Monthly IS NOT NULL AND Monthly = 1 AND (LastMonth IS NULL OR LastMonth > @DateFrom)))
			)
		GROUP BY NameChecksum
	) c
	ON c.Id = e.ID
END
GO

DROP PROCEDURE [expenses].GetIncomeNamesByUser
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 14/12/2015
-- Description:	https://vision.mindjet.com/action/task/14485585
-- =============================================
CREATE PROCEDURE [expenses].GetIncomeNamesByUser
	@DataOwner UNIQUEIDENTIFIER,
	@Year INT, @Month INT, @Day INT,
	@ShortList BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--	Date to start from.
	DECLARE @DateFrom DATETIME
	--	Selects for last half a year.
	SET @DateFrom = DATEADD(month, -6, GETDATE());

	--	https://www.evernote.com/shard/s132/nl/14501366/adbb4c02-3975-460d-88f1-8a65312ca83f
	--	The last expense is selected in each group.
	SELECT e.Name, e.EncryptedName, c.Id
	FROM [expenses].Operations e
	JOIN 
	(
		--	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
		SELECT MAX(e.ID) AS Id
		FROM [expenses].Operations e
		WHERE DataOwner = @DataOwner AND Income IS NOT NULL AND Income = 1
			AND
			(
				@ShortList = 0 OR 
				(Date > @DateFrom OR (Monthly IS NOT NULL AND Monthly = 1 AND (LastMonth IS NULL OR LastMonth > @DateFrom)))
			)
		GROUP BY NameChecksum
	) c
	ON c.Id = e.ID
END
GO
