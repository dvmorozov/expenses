
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 08/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
-- =============================================
CREATE PROCEDURE [expenses].LastYearBalanceByMonthByUser @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	--	For compatibility just the first subset of data is selected.
	--	Set returned by the function already ordered properly, don't use ORDER.
	-- https://github.com/dvmorozov/expenses/issues/28
	SELECT t.Y, t.M, COALESCE(m.Income, 0)- t.Total AS Balance
	FROM 
	(
		SELECT TOP (@LastMonthNumber) *
		--	https://github.com/dvmorozov/expenses/issues/17
		FROM expenses.GetLastYearTotalExpensesByMonthByUser(@LastMonthNumber, @DataOwner) t
		WHERE Currency IS NOT NULL
	) t
	LEFT JOIN Month m
	ON t.Y = m.Year AND t.M = m.Month AND t.Currency = m.Currency
END
