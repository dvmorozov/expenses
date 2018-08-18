
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Create date: 08/06/2015
-- Description:	Returns data for the first currency in the 
--				https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
-- ==========================================================================================
CREATE PROCEDURE [expenses].[LastYearBalanceByMonthByUser] @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP (@LastMonthNumber) Y, M, Balance
	FROM [expenses].[GetLastYearBalanceByMonthByUser](@LastMonthNumber, @DataOwner)
	WHERE Currency IS NOT NULL
END
