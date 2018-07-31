
-- ==========================================================================================
-- Author:		Dmitry Morozov
-- Create date: 21/07/2018
-- Description:	Returns list of last months/years of given length starting from current date.
-- ==========================================================================================

CREATE FUNCTION expenses.GetLastMonthList
(
	@LastMonthNumber INT,
	@StartingDate DATE = NULL
)
RETURNS 
@MonthList TABLE 
(
	Month INT,
	Year INT
)
AS
BEGIN
	DECLARE @Now DATE = COALESCE(@StartingDate, GETDATE())

	WHILE @LastMonthNumber > 0
	BEGIN
		SET @LastMonthNumber = @LastMonthNumber - 1
		INSERT INTO @MonthList
		VALUES (DATEPART(month, @Now), DATEPART(year, @Now))
		SET @Now = DATEADD(mm, -1, @Now)
	END
	RETURN
END
