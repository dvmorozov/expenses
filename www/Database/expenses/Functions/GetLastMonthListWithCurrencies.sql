
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
	DECLARE @Currencies TABLE
	(
		Currency NCHAR(5)
	)
	INSERT INTO @Currencies
	SELECT DISTINCT Currency
	FROM [expenses].Expenses
	WHERE DataOwner = @DataOwner

	INSERT INTO @Result
		SELECT *
		FROM [expenses].GetLastMonthList(@LastMonthNumber)
		CROSS JOIN @Currencies
	RETURN
END
