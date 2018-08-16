
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Create date: 15/08/2018
-- Description:	Generic function for balance stored procedures.
--				https://github.com/dvmorozov/expenses/issues/23
-- ==========================================================================================
CREATE FUNCTION [expenses].[GetLastYearBalanceByMonthByUser]
(
	@LastMonthNumber INT, 
	@DataOwner UNIQUEIDENTIFIER
)
RETURNS @Result TABLE
(
	Y INT,
	M INT,
	Balance REAL,
	Currency NCHAR(5)
)
AS
BEGIN
	INSERT INTO @Result
	-- https://github.com/dvmorozov/expenses/issues/35
	SELECT Y, M, Balance, Currency 
	FROM
	(
		--	For compatibility just the first subset of data is selected.
		--	Set returned by the function already ordered properly, don't use ORDER.
		-- https://github.com/dvmorozov/expenses/issues/28
		SELECT t.Y, t.M, COALESCE(m.Income, 0)- t.Total AS Balance, t.Currency
		FROM 
		(
			SELECT Y, M, Total, Currency
			--	https://github.com/dvmorozov/expenses/issues/17
			FROM [expenses].GetLastYearTotalExpensesByMonthByUser(@LastMonthNumber, @DataOwner) t
		) t
		LEFT JOIN [expenses].Month m
		ON t.Y = m.Year AND t.M = m.Month AND t.Currency = m.Currency
	) t
	ORDER BY Currency, Y, M
	RETURN
END
