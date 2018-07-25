
CREATE PROCEDURE [expenses].MonthTotalByUser2 @Today DATETIME, @DataOwner UNIQUEIDENTIFIER, @MonthTotal FLOAT OUTPUT, @Currency NCHAR(5) OUTPUT
AS
BEGIN
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM [expenses].Month
		WHERE Year = DATEPART(year, @Today) AND Month = DATEPART(month, @Today) AND DataOwner = @DataOwner
	)

	DECLARE @SingleTotal FLOAT

	--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
	SELECT TOP(1) @SingleTotal = Total, @Currency = Currency
	FROM
	(
		SELECT SUM(Cost) AS Total, Currency, COUNT(*) AS CurrencyCount
		FROM
		(
			SELECT Cost,
				CASE WHEN NOT Currency IS NULL AND LEN(TRIM(Currency)) = 0 
				THEN NULL ELSE Currency END AS Currency
			FROM [expenses].Expenses
			WHERE DataOwner = @DataOwner AND 
				(Monthly IS NULL OR Monthly = 0) AND
				DATEPART(YEAR, Date) = DATEPART(YEAR, @Today) AND 
				DATEPART(MONTH, Date) = DATEPART(MONTH, @Today)
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only given currency taken into account in calculating of the total.
				AND (
					Currency = @BudgetCurrency
					-- If not set the currency form separate group.
					OR Currency IS NULL
					-- If the budget currency is not set then all expenses are summed (by groups).
					OR @BudgetCurrency IS NULL
				)
		) T
		--	https://action.mindjet.com/task/14672437
		--	If a few records with different currencies are passed the filters above
		--	then group them by currency and take only the first record (it's best what possibly to do).
		GROUP BY Currency
	) T
	-- The main currency is the currency in which most of expenses were done in the given month.
	ORDER BY CurrencyCount DESC

	DECLARE @RepeatedTotal FLOAT

	--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
	--	Reset @Currency if it wasn't set at the first stage (otherwise it will be set to the same value).
	SELECT TOP(1) @RepeatedTotal = Total, @Currency = Currency
	FROM
	(
		SELECT SUM(Cost) AS Total, Currency, COUNT(*) AS CurrencyCount
		FROM
		(
			SELECT Cost,
				CASE WHEN NOT Currency IS NULL AND LEN(TRIM(Currency)) = 0 
				THEN NULL ELSE Currency END AS Currency
			FROM [expenses].Expenses
			WHERE DataOwner = @DataOwner
				AND (Monthly IS NOT NULL AND Monthly = 1)
				--	FirstMonth and LastMonth correspond to the first day of month.
				AND (@Today >= FirstMonth)
				AND (@Today <= EOMONTH(LastMonth) OR LastMonth IS NULL)
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only given currency taken into account in calculating of the total.
				AND (Currency = @Currency OR @Currency IS NULL)
		) T
		--	If a few records with different currencies are passed the filters above
		--	then group them by currency and take only the first record (it's best what possibly to do).
		GROUP BY Currency
	) T
	-- The main currency is the currency in which most of expenses were done in the given month.
	ORDER BY CurrencyCount DESC

	SELECT @MonthTotal = (
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		SELECT COALESCE(@SingleTotal, 0) + COALESCE(@RepeatedTotal, 0) AS TOTAL
	)
END
