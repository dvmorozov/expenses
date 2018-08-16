
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Create date: 16/08/2018
-- Description:	Returns data for all currencies.
--				https://github.com/dvmorozov/expenses/issues/23
-- ==========================================================================================
CREATE PROCEDURE [expenses].[LastYearBalanceByMonthByUser2]
	@LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	SELECT Y, M, Balance
	FROM [expenses].[GetLastYearBalanceByMonthByUser](@LastMonthNumber, @DataOwner)
END
