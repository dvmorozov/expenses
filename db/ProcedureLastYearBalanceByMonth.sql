
DROP PROCEDURE [expenses].LastYearBalanceByMonthByUser
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 08/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
-- =============================================
CREATE PROCEDURE [expenses].LastYearBalanceByMonthByUser @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	SELECT t.Y, t.M, 
		CASE WHEN m.Income IS NOT NULL THEN m.Income - CAST(t.Total AS MONEY)
		ELSE (-1) * t.Total END AS Balance
	FROM 
	(
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		SELECT Y, M, COALESCE(SingleTotal, 0) + COALESCE(MonthlyTotal, 0) AS Total
		FROM
		(
			SELECT 
				E1.Y, E1.M, 
				E1.Total AS SingleTotal, SUM(E2.Cost) AS MonthlyTotal 
			FROM
			(
				SELECT TOP(@LastMonthNumber) 
					SUM(Cost) AS Total,
					DATEPART(year, Date) AS Y, DATEPART(month, Date) AS M,
					DATEFROMPARTS(DATEPART(year, Date), DATEPART(month, Date), 1) AS CurMonth
				FROM [expenses].Expenses
				WHERE DataOwner = @DataOwner AND Monthly IS NULL OR Monthly = 0
					--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
					--	Only given currency taken into account in calculating of the total.
					AND (Currency = (
							SELECT TOP 1 Currency
							FROM [expenses].Month
							WHERE Year = DATEPART(year, Date) AND Month = DATEPART(month, Date) AND DataOwner = @DataOwner
						)
					-- If not set the currency is considered as it is set for month.
					OR Currency IS NULL
					-- If the budget currency is not set then all expenses are considered as given in the same currency.
					OR (
							SELECT TOP 1 Currency
							FROM [expenses].Month
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
				FROM [expenses].Expenses
				WHERE DataOwner = @DataOwner AND Monthly IS NOT NULL AND Monthly = 1
					--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
					--	Only given currency taken into account in calculating of the total.
					AND (Currency = (
							SELECT TOP 1 Currency
							FROM [expenses].Month
							WHERE Year = DATEPART(year, Date) AND Month = DATEPART(month, Date) AND DataOwner = @DataOwner
						)
					-- If not set the currency is considered as it is set for month.
					OR Currency IS NULL
					-- If the budget currency is not set then all expenses are considered as given in the same currency.
					OR (
							SELECT TOP 1 Currency
							FROM [expenses].Month
							WHERE Year = DATEPART(year, Date) AND Month = DATEPART(month, Date) AND DataOwner = @DataOwner
						) IS NULL)
			) E2
			ON E1.CurMonth >= E2.FirstMonth AND (E1.CurMonth <= E2.LastMonth OR E2.LastMonth IS NULL)
			GROUP BY E1.Y, E1.M, E1.Total
		) AS t
	) AS t 
	LEFT JOIN Month m
	ON t.Y = m.Year AND t.M = m.Month
	ORDER BY t.Y, t.M
END
GO

