
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Create date: 08/01/2018
-- Description:	General function to be used in all stored procedures.
--				https://github.com/dvmorozov/expenses/issues/38
-- ==========================================================================================

CREATE FUNCTION [expenses].[GetEstimatedCategoriesByUser]
(
	@Year INT, 
	@Month INT, 
	@Day INT, 
	@DataOwner UNIQUEIDENTIFIER, 
	@ShortList BIT
)
RETURNS @returntable TABLE
(
	Name CHAR(100), 
	Limit FLOAT, 
	CategoryID INT, 
	CategoryTotal FLOAT,
	Estimation NCHAR(20),
	EncryptedName NVARCHAR(MAX),
	Currency NCHAR(5)
)
AS
BEGIN
	DECLARE @ADate DATETIME
	SET @ADate = DATEFROMPARTS(@Year, @Month, @Day)

	DECLARE @DaysInMonth INT
	SET @DaysInMonth = (SELECT DAY(EOMONTH(@ADate)))

	DECLARE @DaysElapsed INT
	SET @DaysElapsed = (SELECT DATEDIFF(day, DATEFROMPARTS(@Year, @Month, 1), DATEFROMPARTS(@Year, @Month, @Day)) + 1)

	--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM [expenses].Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	--	Date to start from.
	DECLARE @DateFrom DATETIME
	--	Selects for last half a year.
	SET @DateFrom = DATEADD(month, -6, GETDATE());

	WITH CE AS
	(
		SELECT 
			e.ID AS ExpID, c.ID AS CatID, e.Date AS Date, e.Cost AS Cost, c.Name AS CatName, 
			e.Name AS ExpName, c.EncryptedName AS CatEncryptedName, e.EncryptedName AS ExpEncryptedName,
			e.Monthly AS Monthly, e.FirstMonth AS FirstMonth, e.LastMonth AS LastMonth, e.Currency AS Currency
		--	Categories without expenses must be present always 
		--	regardless of value of the ShortList parameter.
		--	https://action.mindjet.com/task/14816150
		FROM [expenses].Categories c
		LEFT JOIN [expenses].ExpensesCategories ec
		ON ec.CategoryID = c.ID 
		LEFT JOIN [expenses].Expenses e
		ON ec.ExpenseID = e.ID
		WHERE c.DataOwner = @DataOwner AND
		(
			(e.ID IS NULL) OR (@ShortList = 0) OR 
			((e.Date > @DateFrom OR (e.Monthly IS NOT NULL AND e.Monthly = 1 AND 
			(e.LastMonth IS NULL OR e.LastMonth > @DateFrom))) AND e.DataOwner = @DataOwner)
		)
	)
	INSERT @returntable
	SELECT 
		LTRIM(RTRIM(CC.NAME)) AS Name, 
		CC.Limit, 
		CC.ID AS CategoryID, 
		TT.TOTAL AS CategoryTotal,
		CASE
			WHEN CC.Limit IS NULL THEN 'NotExceed' 
			WHEN TT.TOTAL > CC.Limit THEN 'Exceed' 
			--	Calculates estimated total for month by the real spend rate.
			WHEN TT.TOTAL > CAST(@DaysElapsed AS FLOAT) * CC.Limit / CAST(@DaysInMonth AS FLOAT) THEN 'MayExceed' ELSE 'NotExceed'
		END AS Estimation,
		CC.EncryptedName,
		TT.Currency
	FROM [expenses].CATEGORIES AS CC 
	-- Selects active categories for given range.
	JOIN 
	(
		SELECT DISTINCT CatID AS ID
		FROM CE
	) AS LastCategories
	ON LastCategories.ID = CC.ID
	LEFT JOIN 
	(
		--	Calculates totals by categories for given month.
		--	Not all categories can be present.
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		SELECT E1.SingleTotal TOTAL, 
			E1.CategoryId AS CATEGORYID,
			E1.Currency
		FROM
		(
			SELECT SUM(Cost) AS SingleTotal, CatID AS CategoryId, Currency
			FROM CE
			WHERE 
				(
					(((Monthly IS NULL OR Monthly = 0) 
						AND DATEPART(YEAR, Date) = @Year AND DATEPART(MONTH, Date) = @Month)
					OR
					((Monthly IS NOT NULL AND Monthly = 1)
						AND @ADate >= FirstMonth AND (@ADate <= LastMonth OR LastMonth IS NULL)))
				--	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
				--	Only budget currency taken into account in calculating of totals.
				AND (Currency = @BudgetCurrency OR (Currency IS NULL AND @BudgetCurrency IS NULL))
				)
			GROUP BY CatID, Currency
		) E1
	) AS TT ON CC.ID=TT.CATEGORYID 
	WHERE CC.DataOwner = @DataOwner

	RETURN
END
