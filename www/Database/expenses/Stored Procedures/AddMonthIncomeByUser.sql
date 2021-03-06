﻿
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 07/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/e9be060a-5343-47e7-9441-65cbb5c80f60
-- =============================================
CREATE PROCEDURE [expenses].AddMonthIncomeByUser @Year INT, @Month INT, @Income MONEY, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @ID INT
	SET @ID = (
		SELECT ID
		FROM [expenses].Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	IF @ID IS NOT NULL
	BEGIN
		--	Updates Income value.
		UPDATE [expenses].Month
		SET Income = Income + @Income
		WHERE ID = @ID
	END
	ELSE
	BEGIN
		--	Inserts new Income value.
		INSERT INTO [expenses].Month (Income, Year, Month, DataOwner)
		VALUES (@Income, @Year, @Month, @DataOwner)
	END
END
