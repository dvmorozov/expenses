
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Last modified: 14/08/2018
-- Description:	For compatibility returns the first part of dataset corresponding to 
--				the first non-NULL currency. 
--				https://github.com/dvmorozov/expenses/issues/54
-- ==========================================================================================
CREATE PROCEDURE [expenses].[LastYearCategoryExpensesByMonthByUser] @CategoryID INT, @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP (@LastMonthNumber)
		Y, M, Total
	FROM [expenses].[GetLastYearCategoryExpensesByMonthByUser](@LastMonthNumber, @DataOwner, @CategoryId)
	WHERE Currency IS NOT NULL
END
