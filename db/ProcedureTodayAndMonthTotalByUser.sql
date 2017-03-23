
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 29/11/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/81118259-b36e-4404-a632-7b140f099b2d
-- =============================================

DROP PROCEDURE MonthTotalByUser
GO

CREATE PROCEDURE MonthTotalByUser @Today DATETIME, @DataOwner UNIQUEIDENTIFIER, @MonthTotal FLOAT OUTPUT
AS
BEGIN
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM Month
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
				FROM Expenses
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
				FROM Expenses
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
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 24/01/2017
-- Description:	https://action.mindjet.com/task/14672437
-- =============================================

DROP PROCEDURE MonthTotalByUser2
GO

CREATE PROCEDURE MonthTotalByUser2 @Today DATETIME, @DataOwner UNIQUEIDENTIFIER, @MonthTotal FLOAT OUTPUT, @Currency NCHAR(5) OUTPUT
AS
BEGIN
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM Month
		WHERE Year = DATEPART(year, @Today) AND Month = DATEPART(month, @Today) AND DataOwner = @DataOwner
	)

	DECLARE @SingleTotal FLOAT

	--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
	SELECT TOP(1) @SingleTotal = Total, @Currency = Currency
	FROM
	(
		SELECT SUM(Cost) AS Total, Currency, COUNT(*) AS CurrencyCount
		FROM Expenses
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
		FROM Expenses
		WHERE DataOwner = @DataOwner
			AND (Monthly IS NOT NULL AND Monthly = 1)
			--	FirstMonth and LastMonth correspond to the first day of month.
			AND (@Today >= FirstMonth)
			AND (@Today <= EOMONTH(LastMonth) OR LastMonth IS NULL)
			--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
			--	Only given currency taken into account in calculating of the total.
			AND (Currency = @Currency OR @Currency IS NULL)
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
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 29/11/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/81118259-b36e-4404-a632-7b140f099b2d
-- =============================================

DROP PROCEDURE TodayAndMonthTotalByUser
GO

CREATE PROCEDURE TodayAndMonthTotalByUser @Today DATETIME, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @Year INT
	DECLARE @Month INT

	SET @Year = DATEPART(year, @Today)
	SET @Month = DATEPART(month, @Today)

	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	DECLARE @TodayTotal FLOAT
	SET @TodayTotal = (
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		SELECT COALESCE(E1.SingleTotal, 0) + COALESCE(E2.MonthlyTotal, 0) AS TOTAL
		FROM
		(
			SELECT SingleTotal, DATEPART(YEAR, @Today) AS Y, DATEPART(MONTH, @Today) AS M, DATEPART(DAY, @Today) AS D
			FROM
			(
				--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
				SELECT TOP(1) SUM(Cost) AS SingleTotal
				FROM Expenses
				WHERE DataOwner = @DataOwner AND 
					(Monthly IS NULL OR Monthly = 0) AND
					DATEPART(YEAR, Date) = DATEPART(YEAR, @Today) AND 
					DATEPART(MONTH, Date) = DATEPART(MONTH, @Today) AND
					DATEPART(DAY, Date) = DATEPART(DAY, @Today)
					--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
					--	Only given currency taken into account in calculating of the total.
					AND (Currency = @BudgetCurrency
						-- If not set the currency is considered as it is set for month.
						OR Currency IS NULL
						-- If the budget currency is not set then all expenses are considered as given in the same currency.
						OR @BudgetCurrency IS NULL
					)
			) T
		) AS E1
		FULL OUTER JOIN
		(
			SELECT MonthlyTotal, DATEPART(YEAR, @Today) AS Y, DATEPART(MONTH, @Today) AS M, DATEPART(DAY, @Today) AS D
			FROM
			(
				--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
				SELECT TOP(1) SUM(Cost) AS MonthlyTotal
				FROM Expenses
				WHERE DataOwner = @DataOwner
					AND (Monthly IS NOT NULL AND Monthly = 1)
					AND DATEPART(DAY, Date) = DATEPART(DAY, @Today)
					--	FirstMonth and LastMonth correspond to the first day of month.
					AND (@Today >= FirstMonth)
					AND (@Today <= EOMONTH(LastMonth) OR LastMonth IS NULL)
					--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
					--	Only given currency taken into account in calculating of the total.
					AND (Currency = @BudgetCurrency
						--	https://www.evernote.com/shard/s132/nl/14501366/7fce5a39-55fc-4419-a2af-6178d77af840
						OR Currency IS NULL
						-- If the budget currency is not set then all expenses are considered as given in the same currency.
						OR @BudgetCurrency IS NULL
					)
			) T
		) AS E2
		ON E1.Y = E2.Y AND E1.M = E2.M AND E1.D = E2.D
	)

	DECLARE @MonthTotal FLOAT
	EXEC MonthTotalByUser2 @Today, @DataOwner, @MonthTotal OUTPUT

	SELECT 
		CASE WHEN @TodayTotal IS NOT NULL THEN @TodayTotal ELSE 0 END AS TodayTotal, 
		CASE WHEN @MonthTotal IS NOT NULL THEN @MonthTotal ELSE 0 END AS MonthTotal
END
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 23/01/2017
-- Description:	https://action.mindjet.com/task/14672437
-- =============================================

DROP PROCEDURE TodayAndMonthTotalByUser2
GO

CREATE PROCEDURE TodayAndMonthTotalByUser2 @Today DATETIME, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @Year INT
	DECLARE @Month INT

	SET @Year = DATEPART(year, @Today)
	SET @Month = DATEPART(month, @Today)

	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM Month
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
		FROM Expenses
		WHERE DataOwner = @DataOwner AND 
			(Monthly IS NULL OR Monthly = 0) AND
			DATEPART(YEAR, Date) = DATEPART(YEAR, @Today) AND 
			DATEPART(MONTH, Date) = DATEPART(MONTH, @Today) AND
			DATEPART(DAY, Date) = DATEPART(DAY, @Today)
			--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
			AND (Currency = @BudgetCurrency
				-- If not set the currency is considered as it is set for month.
				OR Currency IS NULL
				-- If the budget currency is not set then all expenses are considered as given in the same currency.
				OR @BudgetCurrency IS NULL
			)
			--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
			--	Only given currency taken into account in calculating of the total.
			AND (Currency = @BudgetCurrency
				--	https://www.evernote.com/shard/s132/nl/14501366/7fce5a39-55fc-4419-a2af-6178d77af840
				-- If not set the currency form separate group.
				OR Currency IS NULL
				-- If the budget currency is not set then all expenses are summed (by groups).
				OR @BudgetCurrency IS NULL
			)
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
		FROM Expenses
		WHERE DataOwner = @DataOwner
			AND (Monthly IS NOT NULL AND Monthly = 1)
			AND DATEPART(DAY, Date) = DATEPART(DAY, @Today)
			--	FirstMonth and LastMonth correspond to the first day of month.
			AND (@Today >= FirstMonth)
			AND (@Today <= EOMONTH(LastMonth) OR LastMonth IS NULL)
			--	Only currency for non-repeated expenses is taken into account.
			AND (Currency = @TodayCurrency OR @TodayCurrency IS NULL)
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
	EXEC MonthTotalByUser2 @Today, @DataOwner, @MonthTotal OUTPUT, @MonthCurrency OUTPUT

	SELECT 
		CASE WHEN @TodayTotal IS NOT NULL THEN @TodayTotal ELSE 0 END AS TodayTotal,
		CASE WHEN @TodayCurrency IS NOT NULL THEN @TodayCurrency ELSE '' END AS TodayCurrency,
		CASE WHEN @MonthTotal IS NOT NULL THEN @MonthTotal ELSE 0 END AS MonthTotal,
		CASE WHEN @MonthCurrency IS NOT NULL THEN @MonthCurrency ELSE '' END AS MonthCurrency
END
GO
