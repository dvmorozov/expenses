
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Last modified: 22/07/2018
-- Description:	For compatibility returns the first part of dataset corresponding to 
--				the first non-NULL currency. 
-- ==========================================================================================

CREATE PROCEDURE [expenses].LastYearTotalExpensesByMonthByUser @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP (@LastMonthNumber) Y, M, Total
	FROM expenses.GetLastYearTotalExpensesByMonthByUser(@LastMonthNumber, @DataOwner)
	WHERE Currency IS NOT NULL
END
