DROP PROCEDURE [expenses].EstimatedCategoriesByUser3
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 07/12/2015
-- Description:	https://action.mindjet.com/task/14479694
-- =============================================
CREATE PROCEDURE [expenses].EstimatedCategoriesByUser3 @Year INT, @Month INT, @Day INT, @DataOwner UNIQUEIDENTIFIER, @ShortList BIT
AS 
	DECLARE @ADate DATETIME
	SET @ADate = DATEFROMPARTS(@Year, @Month, @Day)

	DECLARE @DaysInMonth INT
	SET @DaysInMonth = (SELECT DAY(EOMONTH(@ADate)))

	DECLARE @DaysElapsed INT
	SET @DaysElapsed = (SELECT DATEDIFF(day, DATEFROMPARTS(@Year, @Month, 1), DATEFROMPARTS(@Year, @Month, @Day)) + 1)

	--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM [expenses].Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	--	Date to start from.
	DECLARE @DateFrom DATETIME
	--	Selects for last half a year.
	SET @DateFrom = DATEADD(month, -6, GETDATE());

	WITH CE AS
	(
		SELECT 
			e.ID AS ExpID, c.ID AS CatID, e.Date AS Date, e.Cost AS Cost, c.Name AS CatName, 
			e.Name AS ExpName, c.EncryptedName AS CatEncryptedName, e.EncryptedName AS ExpEncryptedName,
			e.Monthly AS Monthly, e.FirstMonth AS FirstMonth, e.LastMonth AS LastMonth, e.Currency AS Currency
		--	Categories without expenses must be present always 
		--	regardless of value of the ShortList parameter.
		--	https://action.mindjet.com/task/14816150
		FROM [expenses].Categories c
		LEFT JOIN [expenses].ExpensesCategories ec
		ON ec.CategoryID = c.ID 
		LEFT JOIN [expenses].Expenses e
		ON ec.ExpenseID = e.ID
		WHERE c.DataOwner = @DataOwner AND
		(
			(e.ID IS NULL) OR (@ShortList = 0) OR 
			((e.Date > @DateFrom OR (e.Monthly IS NOT NULL AND e.Monthly = 1 AND 
			(e.LastMonth IS NULL OR e.LastMonth > @DateFrom))) AND e.DataOwner = @DataOwner)
		)
	)
	SELECT LTRIM(RTRIM(CC.NAME)) AS NAME, CC.Limit, CC.ID, TT.Total,
		CASE
			WHEN CC.Limit IS NULL THEN 'NotExceed' 
			WHEN TT.TOTAL > CC.Limit THEN 'Exceed' 
			--	Calculates estimated total for month by the real spend rate.
			WHEN TT.TOTAL > CAST(@DaysElapsed AS FLOAT) * CC.Limit / CAST(@DaysInMonth AS FLOAT) THEN 'MayExceed' ELSE 'NotExceed'
		END AS Estimation,
		CC.EncryptedName	
	FROM [expenses].CATEGORIES AS CC 
	-- Selects active categories for given range.
	JOIN 
	(
		SELECT DISTINCT CatID AS ID
		FROM CE
	) AS LastCategories
	ON LastCategories.ID = CC.ID
	LEFT JOIN 
	(
		--	Calculates totals by categories for given month.
		--	Not all categories can be present.
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		SELECT E1.SingleTotal TOTAL, 
			E1.CategoryId AS CATEGORYID
		FROM
		(
			SELECT SUM(Cost) AS SingleTotal, CatID AS CategoryId
			FROM CE
			WHERE 
				((Monthly IS NULL OR Monthly = 0)
				AND DATEPART(YEAR, Date) = @Year AND DATEPART(MONTH, Date) = @Month 
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only given currency taken into account in calculating of the total.
				AND (Currency = @BudgetCurrency
				-- If not set the currency is considered as it is set for month.
				OR Currency IS NULL
				-- If the budget currency is not set then all expenses are considered as given in the same currency.
				OR @BudgetCurrency IS NULL))
				OR 
				((Monthly IS NOT NULL AND Monthly = 1)
				AND (@ADate >= FirstMonth AND (@ADate <= LastMonth OR LastMonth IS NULL))
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only given currency taken into account in calculating of the total.
				AND (Currency = @BudgetCurrency
				-- If not set the currency is considered as it is set for month.
				OR Currency IS NULL
				-- If the budget currency is not set then all expenses are considered as given in the same currency.
				OR @BudgetCurrency IS NULL)
				)
			GROUP BY CatID
		) E1
	) AS TT ON CC.ID=TT.CATEGORYID 
	WHERE CC.DataOwner = @DataOwner
GO

DROP PROCEDURE [expenses].EstimatedCategoriesByUser2
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 19/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE [expenses].EstimatedCategoriesByUser2 @Year INT, @Month INT, @Day INT, @DataOwner UNIQUEIDENTIFIER 
AS 
	DECLARE @ADate DATETIME
	SET @ADate = DATEFROMPARTS(@Year, @Month, @Day)

	DECLARE @DaysInMonth INT
	SET @DaysInMonth = (SELECT DAY(EOMONTH(@ADate)))

	DECLARE @DaysElapsed INT
	SET @DaysElapsed = (SELECT DATEDIFF(day, DATEFROMPARTS(@Year, @Month, 1), DATEFROMPARTS(@Year, @Month, @Day)) + 1)

	--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM [expenses].Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	SELECT LTRIM(RTRIM(CC.NAME)) AS NAME, CC.Limit, CC.ID, TT.Total,
		CASE
			WHEN CC.Limit IS NULL THEN 'NotExceed' 
			WHEN TT.TOTAL > CC.Limit THEN 'Exceed' 
			--	Calculates estimated total for month by the real spend rate.
			WHEN TT.TOTAL > CAST(@DaysElapsed AS FLOAT) * CC.Limit / CAST(@DaysInMonth AS FLOAT) THEN 'MayExceed' ELSE 'NotExceed'
		END AS Estimation,
		CC.EncryptedName	
	FROM [expenses].CATEGORIES AS CC 
	LEFT JOIN 
	(
		--	Calculates totals by categories for given month.
		--	Not all categories can be present.
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		SELECT COALESCE(E1.SingleTotal, 0) + COALESCE(E2.MonthlyTotal, 0) AS TOTAL, 
			COALESCE(E1.CategoryId, E2.CategoryId) AS CATEGORYID
		FROM
		(
			SELECT SUM(Cost) AS SingleTotal, c.ID AS CategoryId
			FROM [expenses].Expenses e
				JOIN [expenses].ExpensesCategories ec
				ON ec.ExpenseID = e.ID 
				JOIN [expenses].Categories c
				ON ec.CategoryID = c.ID
			WHERE e.DataOwner = @DataOwner 
				AND (Monthly IS NULL OR Monthly = 0)
				AND DATEPART(YEAR, E.Date) = @Year AND DATEPART(MONTH, E.Date) = @Month 
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only given currency taken into account in calculating of the total.
				AND (E.Currency = @BudgetCurrency
				-- If not set the currency is considered as it is set for month.
				OR E.Currency IS NULL
				-- If the budget currency is not set then all expenses are considered as given in the same currency.
				OR @BudgetCurrency IS NULL)
			GROUP BY c.ID
		) E1
		FULL OUTER JOIN
		(
			SELECT SUM(Cost) AS MonthlyTotal, c.ID AS CategoryId
			FROM [expenses].Expenses e
				JOIN [expenses].ExpensesCategories ec
				ON ec.ExpenseID = e.ID 
				JOIN [expenses].Categories c
				ON ec.CategoryID = c.ID
			WHERE e.DataOwner = @DataOwner 
				AND (Monthly IS NOT NULL AND Monthly = 1)
				AND (@ADate >= e.FirstMonth AND (@ADate <= e.LastMonth OR e.LastMonth IS NULL))
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only given currency taken into account in calculating of the total.
				AND (E.Currency = @BudgetCurrency
				-- If not set the currency is considered as it is set for month.
				OR E.Currency IS NULL
				-- If the budget currency is not set then all expenses are considered as given in the same currency.
				OR @BudgetCurrency IS NULL)
			GROUP BY c.ID
		) E2
		ON E1.CategoryId = E2.CategoryId
	) AS TT ON CC.ID=TT.CATEGORYID 
	WHERE CC.DataOwner = @DataOwner
	ORDER BY CC.NAME
GO

DROP PROCEDURE [expenses].EstimatedCategoriesByUser
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 09/03/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/0fc35d9b-80d0-4159-be22-4d935bc2825a
-- =============================================
CREATE PROCEDURE [expenses].EstimatedCategoriesByUser @Year INT, @Month INT, @Day INT, @DataOwner UNIQUEIDENTIFIER 
AS 
	DECLARE @T TABLE (
		NAME CHAR(100) NOT NULL, 
		Limit FLOAT NULL, 
		ID INT NOT NULL, 
		Total FLOAT NULL,
		Estimation NCHAR(20) NOT NULL,
		EncryptedName NVARCHAR(MAX) NULL
		)

	INSERT INTO @T EXEC [expenses].EstimatedCategoriesByUser2 @Year, @Month, @Day, @DataOwner 
	
	SELECT NAME, Limit, ID, Total, Estimation
	FROM @T
GO

