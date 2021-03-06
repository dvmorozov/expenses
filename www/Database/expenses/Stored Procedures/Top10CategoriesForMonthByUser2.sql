﻿
CREATE PROCEDURE [expenses].Top10CategoriesForMonthByUser2 @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM [expenses].Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	SELECT TOP 10 RTRIM(CC.NAME) AS NAME, TT.TOTAL, CC.ID,
		CC.EncryptedName 
	FROM [expenses].CATEGORIES AS CC 
	JOIN 
	(
		SELECT SUM(E.COST) AS TOTAL, C.ID AS CATEGORYID 
		FROM [expenses].EXPENSES AS E 
		JOIN [expenses].EXPENSESCATEGORIES AS EC ON EC.EXPENSEID=E.ID 
		JOIN [expenses].CATEGORIES AS C ON EC.CATEGORYID=C.ID 
		WHERE E.DataOwner = @DataOwner AND 
			--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
			(
				(DATEPART(YEAR, E.Date) = @Year AND DATEPART(MONTH, E.Date) = @Month) 
				OR (E.Monthly IS NOT NULL AND (E.Monthly = 1 
					AND E.FirstMonth <= DATEFROMPARTS(@Year, @Month, 1) 
  					AND (E.LastMonth >= DATEFROMPARTS(@Year, @Month, 1) OR E.LastMonth IS NULL)))
			 )
			--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
			--	Only given currency taken into account in calculating of the total.
			AND (E.Currency = @BudgetCurrency
				-- For monthly expenses we can't do assumption that currency, if not set, is the same as the currency of month budget.
				--OR E.Currency IS NULL
				-- If the budget currency is not set then all expenses are considered as given in the same currency.
				OR @BudgetCurrency IS NULL)
			--	https://www.evernote.com/shard/s132/nl/14501366/f53e1481-b9bc-47f7-a926-4b7011f1a1d9
		GROUP BY C.ID, E.Currency
	) AS TT ON CC.ID=TT.CATEGORYID 
	ORDER BY TT.TOTAL DESC
END
