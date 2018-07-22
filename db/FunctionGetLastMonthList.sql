-- ==================================================================
-- Create Multi-Statement Function template for Azure SQL Database
-- ==================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DROP FUNCTION expenses.GetLastMonthList
GO

-- ==========================================================================================
-- Author:		Dmitry Morozov
-- Create date: 21/07/2018
-- Description:	Returns list of last months/years of given length starting from current date.
-- ==========================================================================================

CREATE FUNCTION expenses.GetLastMonthList
(
	@LastMonthNumber INT
)
RETURNS 
@MonthList TABLE 
(
	Month INT,
	Year INT
)
AS
BEGIN
	DECLARE @Now DATE = GETDATE()
	WHILE @LastMonthNumber > 0
	BEGIN
		SET @LastMonthNumber = @LastMonthNumber - 1
		INSERT INTO @MonthList
		VALUES (DATEPART(month, @Now), DATEPART(year, @Now))
		SET @Now = DATEADD(mm, -1, @Now)
	END
	RETURN
END
GO

DROP FUNCTION expenses.GetLastMonthListWithCurrencies
GO

-- ==========================================================================================
-- Author:		Dmitry Morozov
-- Create date:	22/07/2018
-- Description:	Returns list of last months/years of date given length starting from current 
--				joined with list of all currencies used by given user.
-- ==========================================================================================

CREATE FUNCTION expenses.GetLastMonthListWithCurrencies
(
	@LastMonthNumber INT,
	@DataOwner UNIQUEIDENTIFIER
)
RETURNS 
@ResuLt TABLE 
(
	Month INT,
	Year INT,
	Currency NCHAR(5)
)
AS
BEGIN
	INSERT INTO @Result
		SELECT *
		FROM [expenses].GetLastMonthList(@LastMonthNumber)
		CROSS JOIN
		(
			SELECT DISTINCT Currency
			FROM [expenses].Expenses
			WHERE DataOwner = @DataOwner
		) C
	RETURN
END
GO