
DROP PROCEDURE EstimatedTop10CategoriesForMonthByUser3
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 26/05/2017
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE EstimatedTop10CategoriesForMonthByUser3 @Year int, @Month int, @Day INT, @DataOwner UNIQUEIDENTIFIER 
AS 
	DECLARE @ADate DATETIME
	SET @ADate = DATEFROMPARTS(@Year, @Month, @Day)

	DECLARE @DaysInMonth INT
	SET @DaysInMonth = (SELECT DAY(EOMONTH(@ADate)))

	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	DECLARE @DaysElapsed INT
	SET @DaysElapsed = (SELECT DATEDIFF(day, DATEFROMPARTS(@Year, @Month, 1), DATEFROMPARTS(@Year, @Month, @Day)) + 1)

	SELECT * FROM
	(
		SELECT TT.TOTAL, C.LIMIT, LTRIM(RTRIM(C.NAME)) AS NAME, C.ID,
			CASE
				WHEN LIMIT IS NULL THEN 'NotExceed' 
				WHEN TOTAL > LIMIT THEN 'Exceed' 
				--	Calculates estimated total for month by the real spend rate.
				WHEN TOTAL > CAST(@DaysElapsed AS FLOAT) * LIMIT / CAST(@DaysInMonth AS FLOAT) THEN 'MayExceed' ELSE 'NotExceed'
			END AS ESTIMATION,
			C.EncryptedName,
			TT.Currency,
			DENSE_RANK() OVER (ORDER BY TT.Currency) AS GROUPID1,
			DENSE_RANK() OVER (PARTITION BY TT.Currency ORDER BY TOTAL DESC) AS GROUPID2
		FROM 
		CATEGORIES C
		JOIN
		(
			--	Calculates totals by categories for given month.
			--	Not all categories can be present.
			--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
			SELECT COALESCE(E1.SingleTotal, 0) + COALESCE(E2.MonthlyTotal, 0) AS TOTAL, 
				COALESCE(E1.CategoryId, E2.CategoryId) AS CATEGORYID,
				COALESCE(E1.Currency, E2.Currency) AS CURRENCY
			FROM
			(
				SELECT SUM(Cost) AS SingleTotal, c.ID AS CategoryId, e.Currency AS Currency
				FROM Expenses e
					JOIN ExpensesCategories ec
					ON ec.ExpenseID = e.ID 
					JOIN Categories c
					ON ec.CategoryID = c.ID
				WHERE e.DataOwner = @DataOwner 
					AND (Monthly IS NULL OR Monthly = 0)
					AND DATEPART(YEAR, E.Date) = @Year AND DATEPART(MONTH, E.Date) = @Month 
					--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
					--	Only given currency taken into account in calculating of the total.
					AND (E.Currency = @BudgetCurrency
						-- If not set the currency is considered as it is set for month.
						OR Currency IS NULL
						-- If the budget currency is not set then all expenses are considered as given in the same currency.
						OR @BudgetCurrency IS NULL)
				GROUP BY c.ID, e.Currency
			) E1
			FULL OUTER JOIN
			(
				SELECT SUM(Cost) AS MonthlyTotal, c.ID AS CategoryId, e.Currency AS Currency
				FROM Expenses e
					JOIN ExpensesCategories ec
					ON ec.ExpenseID = e.ID 
					JOIN Categories c
					ON ec.CategoryID = c.ID
				WHERE e.DataOwner = @DataOwner 
					AND (Monthly IS NOT NULL AND Monthly = 1)
					AND (@ADate >= e.FirstMonth AND (@ADate <= e.LastMonth OR e.LastMonth IS NULL))
					--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
					--	Only given currency taken into account in calculating of the total.
					AND (E.Currency = @BudgetCurrency
						--	https://www.evernote.com/shard/s132/nl/14501366/7fce5a39-55fc-4419-a2af-6178d77af840
						OR Currency IS NULL
						-- If the budget currency is not set then all expenses are considered as given in the same currency.
						OR @BudgetCurrency IS NULL)
					--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
				GROUP BY c.ID, e.Currency
			) E2
			ON E1.CategoryId = E2.CategoryId
		) AS TT
		ON C.ID = TT.CATEGORYID
	) TT
	WHERE GROUPID2 < 10
	ORDER BY TT.Currency, TT.TOTAL DESC
GO

DROP PROCEDURE EstimatedTop10CategoriesForMonthByUser2
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 21/05/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE EstimatedTop10CategoriesForMonthByUser2 @Year int, @Month int, @Day INT, @DataOwner UNIQUEIDENTIFIER 
AS 
	DECLARE @T TABLE (
		TOTAL FLOAT NOT NULL,
		LIMIT FLOAT NULL,
		NAME CHAR(100) NOT NULL, 
		ID INT NOT NULL, 
		ESTIMATION NVARCHAR(MAX) NOT NULL, 
		EncryptedName NVARCHAR(MAX) NULL,
		Currency NCHAR(5),
		GROUPID1 INT,
		GROUPID2 INT
		)

	INSERT INTO @T EXEC EstimatedTop10CategoriesForMonthByUser3 @Year, @Month, @Day, @DataOwner

	SELECT TOTAL, LIMIT, NAME, ID, ESTIMATION, EncryptedName
	FROM @T
GO

DROP PROCEDURE EstimatedTop10CategoriesForMonthByUser
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 10/05/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/4bc52f46-5b79-4788-824d-f3a4b0e9fad3
-- =============================================

CREATE PROCEDURE EstimatedTop10CategoriesForMonthByUser @Year int, @Month int, @Day INT, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	DECLARE @T TABLE (
		TOTAL FLOAT NOT NULL,
		LIMIT FLOAT NULL,
		NAME CHAR(100) NOT NULL, 
		ID INT NOT NULL, 
		ESTIMATION NVARCHAR(MAX) NOT NULL, 
		EncryptedName NVARCHAR(MAX) NULL,
		Currency NCHAR(5),
		GROUPID1 INT,
		GROUPID2 INT
		)

	INSERT INTO @T EXEC EstimatedTop10CategoriesForMonthByUser3 @Year, @Month, @Day, @DataOwner

	SELECT TOTAL, LIMIT, NAME, ID, ESTIMATION
	FROM @T
END
