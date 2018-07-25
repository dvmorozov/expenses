
CREATE PROCEDURE [expenses].TodayAndMonthTotalByUser2 @Today DATETIME, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @Year INT
	DECLARE @Month INT

	SET @Year = DATEPART(year, @Today)
	SET @Month = DATEPART(month, @Today)

	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM [expenses].Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	DECLARE @SingleTotal FLOAT
	DECLARE @TodayCurrency NCHAR(5)

	--	Get single day totals for non repeated expenses.
	--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
	SELECT TOP(1) @SingleTotal = Total, @TodayCurrency = Currency
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
				DATEPART(MONTH, Date) = DATEPART(MONTH, @Today) AND
				DATEPART(DAY, Date) = DATEPART(DAY, @Today)
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only given currency taken into account in calculating of the total.
				AND (Currency = @BudgetCurrency
					--	https://www.evernote.com/shard/s132/nl/14501366/7fce5a39-55fc-4419-a2af-6178d77af840
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

	--	Get totals for repeated expenses for given day.
	--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
	--	Reset @TodayCurrency if it wasn't set at the first stage (otherwise it will be set to the same value).
	SELECT TOP(1) @RepeatedTotal = Total, @TodayCurrency = Currency
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
				AND DATEPART(DAY, Date) = DATEPART(DAY, @Today)
				--	FirstMonth and LastMonth correspond to the first day of month.
				AND (@Today >= FirstMonth)
				AND (@Today <= EOMONTH(LastMonth) OR LastMonth IS NULL)
				--	Only currency for non-repeated expenses is taken into account.
				AND (Currency = @TodayCurrency OR @TodayCurrency IS NULL)
		) T
		--	If a few records with different currencies are passed the filters above
		--	then group them by currency and take only the first record (it's best what possibly to do).
		GROUP BY Currency
	) T
	-- The main currency is the currency in which most of expenses were done in the given month.
	ORDER BY CurrencyCount DESC

	DECLARE @TodayTotal FLOAT

	SET @TodayTotal = (
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		SELECT COALESCE(@SingleTotal, 0) + COALESCE(@RepeatedTotal, 0) AS TOTAL
	)

	DECLARE @MonthTotal FLOAT
	DECLARE @MonthCurrency NCHAR(5)
	EXEC [expenses].MonthTotalByUser2 @Today, @DataOwner, @MonthTotal OUTPUT, @MonthCurrency OUTPUT

	SELECT 
		CASE WHEN @TodayTotal IS NOT NULL THEN @TodayTotal ELSE 0 END AS TodayTotal,
		CASE WHEN @TodayCurrency IS NOT NULL THEN @TodayCurrency ELSE '' END AS TodayCurrency,
		CASE WHEN @MonthTotal IS NOT NULL THEN @MonthTotal ELSE 0 END AS MonthTotal,
		CASE WHEN @MonthCurrency IS NOT NULL THEN @MonthCurrency ELSE '' END AS MonthCurrency
END
