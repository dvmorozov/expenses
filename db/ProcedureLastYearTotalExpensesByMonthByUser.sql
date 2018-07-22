
DROP PROCEDURE [expenses].LastYearTotalExpensesByMonthByUser
GO

-- ==========================================================================================
-- Author:		D.V.Morozov
-- Last modified: 22/07/2018
-- Description:	For compatibility returns the first part of dataset corresponding to 
--				the first non-NULL currency. 
-- ==========================================================================================

CREATE PROCEDURE [expenses].LastYearTotalExpensesByMonthByUser @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @T TABLE (
		--	Must allow NULLs. https://github.com/dvmorozov/expenses/issues/16
		Y INT, 
		M INT,
		Total FLOAT NOT NULL, 
		Month NVARCHAR(10) NULL,
		Currency NCHAR(5)
		)

	-- All parts must have the same size equal to @LastMonthNumber, 
	-- the dataset must be ordered by currency at first.
	INSERT INTO @T EXEC [expenses].LastYearTotalExpensesByMonthByUser2 @LastMonthNumber, @DataOwner

	SELECT TOP (@LastMonthNumber) Y, M, Total
	FROM @T
	WHERE Currency IS NOT NULL
END
GO

DROP PROCEDURE [expenses].LastYearTotalExpensesByMonthByUser2
GO

-- ==========================================================================================
-- Author:		D.V.Morozov
-- Last modified: 18/07/2018
-- Description:	Additionally to LastYearTotalExpensesByMonthByUser returns Month as text 
--				and Currency. Returns records for all currencies in the single dataset.
--				All parts must have the same size equal to @LastMonthNumber, the dataset
--				must be ordered by currency at first.
-- ==========================================================================================

CREATE PROCEDURE [expenses].LastYearTotalExpensesByMonthByUser2 @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
	SELECT 
		Y,
		M,
		COALESCE(SingleTotal, 0) + COALESCE(MonthlyTotal, 0) AS Total,
		CAST(Y AS VARCHAR) + '/' + 
		CASE WHEN LEN(CAST(M AS VARCHAR)) < 2 THEN '0' + CAST(M AS VARCHAR)
		ELSE CAST(M AS VARCHAR) END AS Month, Currency
	FROM
	(
		SELECT 
			COALESCE(E1.Y, E2.Y) AS Y,
			COALESCE(E1.M, E2.M) AS M,
			COALESCE(SingleTotal, 0) AS SingleTotal,
			COALESCE(MonthlyTotal, 0) AS MonthlyTotal,
			COALESCE(E1.Currency, E2.Currency) AS Currency
		FROM
		(
			-- Selects single-time expenses.
			SELECT 
				Year AS Y, Month AS M, 
				E1.Currency, 
				SUM(Cost) AS SingleTotal
			FROM [expenses].GetLastMonthListWithCurrencies(@LastMonthNumber, @DataOwner) E1
			LEFT OUTER JOIN
			(
				SELECT Cost, Currency, Date
				FROM [expenses].Expenses
				WHERE DataOwner = @DataOwner AND (Monthly IS NULL OR Monthly = 0)
			) E2
			ON Year = DATEPART(year, E2.Date) 
				AND Month = DATEPART(month, E2.Date) 
				AND (E1.Currency = E2.Currency)
			GROUP BY Year, Month, E1.Currency
		) E1
		FULL OUTER JOIN
		(
			--	Selects montly expenses.
			SELECT Year AS Y, Month AS M, SUM(Cost) AS MonthlyTotal, E1.Currency
			FROM [expenses].GetLastMonthListWithCurrencies(@LastMonthNumber, @DataOwner) E1
			LEFT JOIN
			(
				SELECT Cost, FirstMonth, LastMonth, Currency
				FROM [expenses].Expenses
				WHERE DataOwner = @DataOwner AND Monthly IS NOT NULL AND Monthly = 1
			) E2
			ON DATEFROMPARTS(Year, Month, 1) >= E2.FirstMonth 
				AND (DATEFROMPARTS(Year, Month, 1) <= E2.LastMonth OR E2.LastMonth IS NULL) 
				AND (E1.Currency = E2.Currency)
			GROUP BY Year, Month, E1.Currency
		) E2
		ON E1.Y = E2.Y AND E1.M = E2.M AND (E1.Currency = E2.Currency OR E1.Currency IS NULL AND E2.Currency IS NULL)
	)
	AS T
	ORDER BY Currency, Y, M
END
GO
