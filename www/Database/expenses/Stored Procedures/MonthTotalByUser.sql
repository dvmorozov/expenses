
CREATE PROCEDURE [expenses].MonthTotalByUser @Today DATETIME, @DataOwner UNIQUEIDENTIFIER, @MonthTotal FLOAT OUTPUT
AS
BEGIN
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM [expenses].Month
		WHERE Year = DATEPART(year, @Today) AND Month = DATEPART(month, @Today) AND DataOwner = @DataOwner
	)

	SELECT @MonthTotal = (
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		SELECT COALESCE(E1.SingleTotal, 0) + COALESCE(E2.MonthlyTotal, 0) AS TOTAL
		FROM
		(
			SELECT SingleTotal, DATEPART(YEAR, @Today) AS Y, DATEPART(MONTH, @Today) AS M
			FROM
			(
				--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
				SELECT TOP(1) SUM(Cost) AS SingleTotal
				FROM [expenses].Expenses
				WHERE DataOwner = @DataOwner AND 
					(Monthly IS NULL OR Monthly = 0) AND
					DATEPART(YEAR, Date) = DATEPART(YEAR, @Today) AND 
					DATEPART(MONTH, Date) = DATEPART(MONTH, @Today)
					--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
					--	Only given currency taken into account in calculating of the total.
					AND (
						Currency = @BudgetCurrency
						-- If not set the currency is considered as it is set for month.
						OR Currency IS NULL
						-- If the budget currency is not set then all expenses are considered as given in the same currency.
						OR @BudgetCurrency IS NULL
					)
			) T
		) AS E1
		FULL OUTER JOIN
		(
			SELECT MonthlyTotal, DATEPART(YEAR, @Today) AS Y, DATEPART(MONTH, @Today) AS M
			FROM
			(
				--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
				SELECT TOP(1) SUM(Cost) AS MonthlyTotal
				FROM [expenses].Expenses
				WHERE DataOwner = @DataOwner
					AND (Monthly IS NOT NULL AND Monthly = 1)
					--	FirstMonth and LastMonth correspond to the first day of month.
					AND (@Today >= FirstMonth)
					AND (@Today <= EOMONTH(LastMonth) OR LastMonth IS NULL)
					--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
					--	Only given currency taken into account in calculating of the total.
					AND (
						Currency = @BudgetCurrency
						--	https://www.evernote.com/shard/s132/nl/14501366/7fce5a39-55fc-4419-a2af-6178d77af840
						OR Currency IS NULL
						-- If the budget currency is not set then all expenses are considered as given in the same currency.
						OR @BudgetCurrency IS NULL
					)
			) T
		) AS E2
		ON E1.Y = E2.Y AND E1.M = E2.M
	)
END
