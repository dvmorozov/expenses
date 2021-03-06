﻿
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 26/05/2017
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE [expenses].EstimatedTop10CategoriesForMonthByUser3 @Year int, @Month int, @Day INT, @DataOwner UNIQUEIDENTIFIER 
AS 
	DECLARE @ADate DATETIME
	SET @ADate = DATEFROMPARTS(@Year, @Month, @Day)

	DECLARE @DaysInMonth INT
	SET @DaysInMonth = (SELECT DAY(EOMONTH(@ADate)))

	DECLARE @BudgetCurrency NCHAR(5)
	--  Select budget currency if it was set for given month.
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM [expenses].Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	DECLARE @DaysElapsed INT
	SET @DaysElapsed = (SELECT DATEDIFF(day, DATEFROMPARTS(@Year, @Month, 1), DATEFROMPARTS(@Year, @Month, @Day)) + 1)

	SELECT TOTAL, LIMIT, NAME, ID, ESTIMATION, EncryptedName, Currency, GROUPID1, GROUPID2 FROM
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
		[expenses].CATEGORIES C
		JOIN
		(
			--	Calculates totals by categories for given month.
			--	Not all categories can be present.
			--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
			--	Sums totals for repeated and non-repeated expenses for given category.
			SELECT COALESCE(E1.SingleTotal, 0) + COALESCE(E2.MonthlyTotal, 0) AS TOTAL, 
				COALESCE(E1.CategoryId, E2.CategoryId) AS CATEGORYID,
				COALESCE(E1.Currency, E2.Currency) AS CURRENCY
			FROM
			(
				SELECT SUM(Cost) AS SingleTotal, c.ID AS CategoryId, e.Currency AS Currency
				FROM [expenses].Expenses e
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
				FROM [expenses].Expenses e
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
			--	https://action.mindjet.com/task/14968958
			--	Expenses should be joined by category and currency in the case when budget currency isn't given.
			ON E1.CategoryId = E2.CategoryId AND E1.Currency = E2.Currency
		) AS TT
		ON C.ID = TT.CATEGORYID
	) TT
	WHERE GROUPID2 <= 10
	ORDER BY TT.Currency, TT.TOTAL DESC
