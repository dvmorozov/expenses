
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Last modified: 20/08/2018
-- Description:	Returns data for all currencies.
--				https://github.com/dvmorozov/expenses/issues/23
-- ==========================================================================================

CREATE PROCEDURE [expenses].[LastYearCategoryExpensesByMonthByUser2] @CategoryID INT, @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	SELECT Y, M, Total, Month, Currency
	FROM [expenses].[GetLastYearCategoryExpensesByMonthByUser](@LastMonthNumber, @DataOwner, @CategoryId)
END
