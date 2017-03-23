
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DROP PROCEDURE MonthBudgetByUser
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 06/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/14e369f7-348f-4f68-aa65-6a5e7dda1da7
-- =============================================
CREATE PROCEDURE MonthBudgetByUser @Today DATETIME, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @Year INT
	SET @Year = DATEPART(YEAR, @Today)

	DECLARE @Month INT
	SET @Month = DATEPART(MONTH, @Today)

	DECLARE @DaysElapsed INT
	SET @DaysElapsed = DATEPART(DAY, @Today)

	DECLARE @DaysInMonth INT
	SET @DaysInMonth = (SELECT DAY(EOMONTH(@Today)))

	DECLARE @BudgetCurrency NCHAR(5)
	SET @BudgetCurrency = (
		SELECT TOP 1 Currency
		FROM Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	--	https://www.evernote.com/shard/s132/nl/14501366/81118259-b36e-4404-a632-7b140f099b2d
	DECLARE @MonthTotal MONEY
	EXEC MonthTotalByUser @Today, @DataOwner, @MonthTotal OUTPUT

	DECLARE @MonthBudget MONEY
	SET @MonthBudget = (
		SELECT TOP 1 Budget
		FROM Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	SELECT @MonthBudget AS MonthBudget, @MonthTotal AS MonthTotal, 
		CASE 
			WHEN @MonthBudget IS NULL OR @MonthTotal IS NULL THEN 'NotExceed' 
			WHEN @MonthTotal > @MonthBudget THEN 'Exceed' 
			--	Calculates estimated total for month by the real spend rate.
			WHEN @MonthTotal > CAST(@DaysElapsed AS FLOAT) * @MonthBudget / CAST(@DaysInMonth AS FLOAT) THEN 'MayExceed' ELSE 'NotExceed'
		END AS Estimation
END
GO
