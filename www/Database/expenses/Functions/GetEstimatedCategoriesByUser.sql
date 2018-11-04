
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Create date: 08/01/2018
-- Description:	General function to be used in all stored procedures.
--				https://github.com/dvmorozov/expenses/issues/38
-- ==========================================================================================

CREATE FUNCTION [expenses].[GetEstimatedCategoriesByUser]
(
	@Year INT, 
	@Month INT, 
	@Day INT, 
	@DataOwner UNIQUEIDENTIFIER, 
	@ShortList BIT
)
RETURNS @returntable TABLE
(
	Name CHAR(100), 
	Limit FLOAT, 
	CategoryID INT, 
	CategoryTotal FLOAT,
	Estimation NCHAR(20),
	EncryptedName NVARCHAR(MAX),
	Currency NCHAR(5)
)
AS
BEGIN
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
	--	Selects for last 3 months from given date.
	--  https://github.com/dvmorozov/expenses/issues/122
	SET @DateFrom = DATEADD(month, -3, @ADate);

	--	Selects categories having expenses for given period of time.
	WITH CategoriesExpenses AS
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
			--	Selects categories for all the time.
			(@ShortList = 0) OR 
			--	Selects categories for last period of time.
			(
				--	Selects one-time expenses.
				(e.Date > @DateFrom AND (e.Monthly IS NULL OR e.Monthly = 0))
				OR 
				--	Selects montly expenses which are still actual for given period of time.
				(e.Monthly IS NOT NULL AND e.Monthly = 1 AND 
				(e.LastMonth IS NULL OR e.LastMonth > @DateFrom))
			)
		)
	)
	INSERT @returntable
	SELECT 
		LTRIM(RTRIM(CC.NAME)) AS Name, 
		CC.Limit, 
		CC.ID AS CategoryID, 
		TT.TOTAL AS CategoryTotal,
		CASE
			WHEN CC.Limit IS NULL THEN 'NotExceed' 
			WHEN TT.TOTAL > CC.Limit THEN 'Exceed' 
			--	Calculates estimated total for month by the real spend rate.
			WHEN TT.TOTAL > CAST(@DaysElapsed AS FLOAT) * CC.Limit / CAST(@DaysInMonth AS FLOAT) THEN 'MayExceed' ELSE 'NotExceed'
		END AS Estimation,
		CC.EncryptedName,
		TT.Currency
	FROM [expenses].CATEGORIES AS CC 
	-- Selects active categories for given range
	-- (includes categories even they don't have related expenses).
	JOIN 
	(
		SELECT DISTINCT CatID AS ID
		FROM CategoriesExpenses
	) AS LastCategories
	ON LastCategories.ID = CC.ID
	LEFT JOIN 
	(
		-- Selects only the first group, preferably meeting budget currency.
		SELECT *
		FROM
		(
			--	Calculates totals by categories for given month.
			--	Not all categories can be present.
			--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
			SELECT E1.SingleTotal TOTAL, 
				E1.CategoryId AS CATEGORYID,
				E1.Currency,
				DENSE_RANK() OVER (PARTITION BY CategoryId ORDER BY MeetBudget DESC, Currency) AS GroupNumber
			FROM
			(
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Selects the first total by currency, meeting budget currency if possible.
				SELECT
					SUM(Cost) AS SingleTotal, CatID AS CategoryId, Currency,
					CASE WHEN Currency = @BudgetCurrency THEN 1 ELSE 0 END AS MeetBudget
				FROM CategoriesExpenses
				WHERE 
					(
						(
							--	Selects one-time expenses for given month.
							(
								(Monthly IS NULL OR Monthly = 0) 
									AND DATEPART(YEAR, Date) = @Year AND DATEPART(MONTH, Date) = @Month
							)
							OR
							(
								--	Selects monthly expenses for given month.
								Monthly IS NOT NULL AND Monthly = 1
									AND (@ADate >= FirstMonth)
									AND (@ADate <= LastMonth OR LastMonth IS NULL)
							)
						)
					)
				GROUP BY CatID, Currency
			) E1
		) G
		WHERE G.GroupNumber = 1
	) AS TT ON CC.ID=TT.CATEGORYID 
	WHERE CC.DataOwner = @DataOwner
	RETURN
END
