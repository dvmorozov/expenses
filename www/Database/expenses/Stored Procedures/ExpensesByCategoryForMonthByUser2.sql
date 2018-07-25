
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 21/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE [expenses].ExpensesByCategoryForMonthByUser2 @CategoryId int, @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM [expenses].Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	SELECT LTRIM(RTRIM(TT.NAME)) AS NAME, TT.TOTAL, 
		TT.EncryptedName
	FROM 
	(
		SELECT SUM(E.COST) AS TOTAL, E.Name AS NAME,
			E.EncryptedName
		FROM [expenses].EXPENSES AS E 
		JOIN [expenses].EXPENSESCATEGORIES AS EC ON EC.EXPENSEID=E.ID 
		JOIN [expenses].CATEGORIES AS C ON EC.CATEGORYID=C.ID 
		WHERE E.DataOwner = @DataOwner AND DATEPART(YEAR, DATE) = @YEAR AND DATEPART(MONTH, DATE) = @MONTH 
			AND C.ID = @CategoryId
			--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
			--	Only given currency taken into account in calculating of the total.
			AND (E.Currency = @BudgetCurrency
			-- If not set the currency is considered as it is set for month.
			OR E.Currency IS NULL
			-- If the budget currency is not set then all expenses are considered as given in the same currency.
			OR @BudgetCurrency IS NULL)
		GROUP BY E.Name, E.EncryptedName
	) AS TT
	ORDER BY TT.TOTAL DESC
END
