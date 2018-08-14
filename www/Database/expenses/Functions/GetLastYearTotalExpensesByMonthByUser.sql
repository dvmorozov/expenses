
-- =============================================
-- Author:		Dmitry Morozov
-- Create date:	23.07.2018
-- Description:	Returns total amount of expenses by month for given number of last months.
--				Additionally returns Month as text and Currency. Returns records for all 
--				currencies in the single dataset.
--				All parts must have the same size equal to @LastMonthNumber, the dataset
--				must be ordered by currency at first.
-- =============================================
CREATE FUNCTION [expenses].[GetLastYearTotalExpensesByMonthByUser]
(
	@LastMonthNumber INT, 
	@DataOwner UNIQUEIDENTIFIER
)
RETURNS 
@Result TABLE 
(
	--	Mustn't allow NULLs. https://github.com/dvmorozov/expenses/issues/17
	Y INT NOT NULL, 
	M INT NOT NULL,
	Total FLOAT NOT NULL, 
	Month NVARCHAR(10) NULL,
	Currency NCHAR(5)
)
AS
BEGIN
	--  https://github.com/dvmorozov/expenses/issues/54
	INSERT INTO @Result
	SELECT Y, M, Total, Month, Currency
	FROM [expenses].[GetLastYearCategoryExpensesByMonthByUser](@LastMonthNumber, @DataOwner, NULL)
	RETURN 
END
