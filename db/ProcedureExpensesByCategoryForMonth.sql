
DROP PROCEDURE ExpensesByCategoryForMonthByUser2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 21/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE ExpensesByCategoryForMonthByUser2 @CategoryId int, @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	SELECT LTRIM(RTRIM(TT.NAME)) AS NAME, TT.TOTAL, 
		TT.EncryptedName
	FROM 
	(
		SELECT SUM(E.COST) AS TOTAL, E.Name AS NAME,
			E.EncryptedName
		FROM EXPENSES AS E 
		JOIN EXPENSESCATEGORIES AS EC ON EC.EXPENSEID=E.ID 
		JOIN CATEGORIES AS C ON EC.CATEGORYID=C.ID 
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
GO

DROP PROCEDURE ExpensesByCategoryForMonthByUser
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 22/03/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/751d5935-68c5-42be-8f12-c5ab2315da02
-- =============================================
CREATE PROCEDURE ExpensesByCategoryForMonthByUser @CategoryId int, @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	DECLARE @T TABLE (
		NAME NCHAR(50) NOT NULL, 
		TOTAL FLOAT NOT NULL, 
		EncryptedName NVARCHAR(MAX) NULL
		)

	INSERT INTO @T EXEC ExpensesByCategoryForMonthByUser2 @CategoryId, @Year, @Month, @DataOwner

	SELECT NAME, TOTAL
	FROM @T
END
GO

DROP PROCEDURE IncomsForMonthByUser
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 13/12/2015
-- Description:	https://vision.mindjet.com/action/task/14485575
-- =============================================
CREATE PROCEDURE IncomsForMonthByUser @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	SELECT ID, Date, Cost AS Amount, Name, EncryptedName
	FROM Operations
	WHERE DataOwner = @DataOwner AND Income IS NOT NULL AND Income = 1 AND DATEPART(YEAR, DATE) = @YEAR AND DATEPART(MONTH, DATE) = @MONTH 
	ORDER BY Date ASC
END
GO
