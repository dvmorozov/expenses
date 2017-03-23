
DROP PROCEDURE LastYearTotalExpensesByMonthByUser
GO

CREATE PROCEDURE LastYearTotalExpensesByMonthByUser @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @T TABLE (
		Y INT NOT NULL, 
		M INT NOT NULL,
		Total FLOAT NOT NULL, 
		Month NVARCHAR(10) NULL
		)

	INSERT INTO @T EXEC LastYearTotalExpensesByMonthByUser2 @LastMonthNumber, @DataOwner

	SELECT Y, M, Total
	FROM @T
END
GO

DROP PROCEDURE LastYearTotalExpensesByMonthByUser2
GO

CREATE PROCEDURE LastYearTotalExpensesByMonthByUser2 @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
	SELECT Y, M, COALESCE(SingleTotal, 0) + COALESCE(MonthlyTotal, 0) AS Total,
		CAST(Y AS VARCHAR) + '/' + 
		CASE WHEN LEN(CAST(M AS VARCHAR)) < 2 THEN '0' + CAST(M AS VARCHAR)
		ELSE CAST(M AS VARCHAR) END AS Month
	FROM
	(
		SELECT E1.Y, E1.M, E1.Total AS SingleTotal, SUM(E2.Cost) AS MonthlyTotal FROM
		(
				SELECT TOP(@LastMonthNumber) 
					SUM(Cost) AS Total,
					DATEPART(year, Date) AS Y, DATEPART(month, Date) AS M,
					DATEFROMPARTS(DATEPART(year, Date), DATEPART(month, Date), 1) AS CurMonth
				FROM Expenses
				WHERE DataOwner = @DataOwner AND Monthly IS NULL OR Monthly = 0
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only given currency taken into account in calculating of the total.
				AND (Currency = (
						SELECT TOP 1 Currency
						FROM Month
						WHERE Year = DATEPART(year, Date) AND Month = DATEPART(month, Date) AND DataOwner = @DataOwner
					)
				-- If not set the currency is considered as it is set for month.
				OR Currency IS NULL
				-- If the budget currency is not set then all expenses are considered as given in the same currency.
				OR (
						SELECT TOP 1 Currency
						FROM Month
						WHERE Year = DATEPART(year, Date) AND Month = DATEPART(month, Date) AND DataOwner = @DataOwner
					) IS NULL)
				GROUP BY DATEPART(year, Date), DATEPART(month, Date)
				--	Сортировка д. б. здесь, чтобы создать правильный рейтинг TOP.
				ORDER BY Y DESC, M DESC
		) E1
		FULL OUTER JOIN
		(
			SELECT Cost, FirstMonth, LastMonth,
				DATEPART(year, Date) AS Y, DATEPART(month, Date) AS M
			FROM Expenses
			WHERE DataOwner = @DataOwner AND Monthly IS NOT NULL AND Monthly = 1
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only given currency taken into account in calculating of the total.
				AND (Currency = (
						SELECT TOP 1 Currency
						FROM Month
						WHERE Year = DATEPART(year, Date) AND Month = DATEPART(month, Date) AND DataOwner = @DataOwner
					)
				-- If not set the currency is considered as it is set for month.
				OR Currency IS NULL
				-- If the budget currency is not set then all expenses are considered as given in the same currency.
				OR (
						SELECT TOP 1 Currency
						FROM Month
						WHERE Year = DATEPART(year, Date) AND Month = DATEPART(month, Date) AND DataOwner = @DataOwner
					) IS NULL)
		) E2
		ON E1.CurMonth >= E2.FirstMonth AND (E1.CurMonth <= E2.LastMonth OR E2.LastMonth IS NULL)
		GROUP BY E1.Y, E1.M, E1.Total
	)
	AS T
END
GO
