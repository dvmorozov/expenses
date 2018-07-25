
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 06/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/14e369f7-348f-4f68-aa65-6a5e7dda1da7
-- =============================================
CREATE PROCEDURE [expenses].AddMonthBudgetByUser @Year INT, @Month INT, @Budget MONEY, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	EXEC [expenses].AddMonthBudgetByUser2 @Year, @Month, @Budget, @DataOwner, NULL
END
