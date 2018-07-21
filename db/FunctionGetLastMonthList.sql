-- ==================================================================
-- Create Multi-Statement Function template for Azure SQL Database
-- ==================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DROP FUNCTION expenses.GetLastMonthList
GO

-- =============================================
-- Author:		Dmitry Morozov
-- Create date: 21/07/2018
-- Description:	Returns list of last months/years of 
--				given length starting from current date.
-- =============================================
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
	-- Fill the table variable with the rows for your result set
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
