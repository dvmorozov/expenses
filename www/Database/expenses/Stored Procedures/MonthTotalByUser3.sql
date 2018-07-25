
CREATE PROCEDURE [expenses].MonthTotalByUser3 @Today DATETIME, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	SELECT SUM(Cost) AS Total, Currency
	FROM
	(
		SELECT Cost,
			CASE WHEN NOT Currency IS NULL AND LEN(TRIM(Currency)) = 0 
			THEN NULL ELSE Currency END AS Currency
		FROM [expenses].Expenses
		WHERE DataOwner = @DataOwner AND (
			(
				(Monthly IS NULL OR Monthly = 0) AND
				DATEPART(YEAR, Date) = DATEPART(YEAR, @Today) AND 
				DATEPART(MONTH, Date) = DATEPART(MONTH, @Today)
			)
			OR
			(
				(Monthly IS NOT NULL AND Monthly = 1) AND 
				--	FirstMonth and LastMonth correspond to the first day of month.
				(@Today >= FirstMonth) AND 
				(@Today <= EOMONTH(LastMonth) OR LastMonth IS NULL)
			)
		)
	) T
	GROUP BY Currency
END
