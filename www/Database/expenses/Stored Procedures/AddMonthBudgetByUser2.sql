
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 23/07/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
-- =============================================
CREATE PROCEDURE [expenses].AddMonthBudgetByUser2 @Year INT, @Month INT, @Budget MONEY, @DataOwner UNIQUEIDENTIFIER, @Currency NCHAR(5)
AS
BEGIN
	DECLARE @ID INT
	SET @ID = (
		SELECT ID
		FROM Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	IF @ID IS NOT NULL
	BEGIN
		--	Updates budget value.
		UPDATE [expenses].Month
		SET Budget = @Budget, Currency = @Currency
		WHERE ID = @ID
	END
	ELSE
	BEGIN
		--	Inserts new budget value.
		INSERT INTO [expenses].Month (Budget, Year, Month, DataOwner, Currency)
		VALUES (@Budget, @Year, @Month, @DataOwner, @Currency)
	END
END
