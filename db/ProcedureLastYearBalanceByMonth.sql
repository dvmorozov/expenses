
DROP PROCEDURE [expenses].LastYearBalanceByMonthByUser
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 08/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
-- =============================================
CREATE PROCEDURE [expenses].LastYearBalanceByMonthByUser @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP (@LastMonthNumber) t.Y, t.M, 
		COALESCE(m.Income, 0)- t.Total AS Balance
	--  https://github.com/dvmorozov/expenses/issues/17
	FROM expenses.GetLastYearTotalExpensesByMonthByUser(@LastMonthNumber, @DataOwner) t
	LEFT JOIN Month m
	ON t.Y = m.Year AND t.M = m.Month
	ORDER BY t.Y, t.M
END
GO

