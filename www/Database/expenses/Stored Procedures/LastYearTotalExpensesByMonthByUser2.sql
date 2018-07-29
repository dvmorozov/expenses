
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Last modified: 18/07/2018
-- Description:	Additionally to LastYearTotalExpensesByMonthByUser returns Month as text 
--				and Currency. Returns records for all currencies in the single dataset.
--				All parts must have the same size equal to @LastMonthNumber, the dataset
--				must be ordered by currency at first.
-- ==========================================================================================

CREATE PROCEDURE [expenses].LastYearTotalExpensesByMonthByUser2 @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	SELECT *
	FROM expenses.GetLastYearTotalExpensesByMonthByUser(@LastMonthNumber, @DataOwner)
END
