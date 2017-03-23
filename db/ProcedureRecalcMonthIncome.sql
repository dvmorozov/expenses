
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DROP PROCEDURE RecalcMonthIncome
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 18/12/2015
-- Description:	https://vision.mindjet.com/action/task/14486189
-- =============================================

CREATE PROCEDURE RecalcMonthIncome @Year INT, @Month INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @Currency NCHAR(5)
	SET @Currency = (
		SELECT Currency FROM Month 
		WHERE Year = @Year AND Month = @Month 
			AND DataOwner = @DataOwner
		)

	DECLARE @TotalIncome MONEY
	SET @TotalIncome = (
		SELECT TOP 1 SUM(Cost)
		FROM Operations
		WHERE 
			Income IS NOT NULL AND Income = 1
			AND DATEPART(year, Date) = @Year AND DATEPART(month, Date) = @Month
			AND DataOwner = @DataOwner
			AND 
			(
				Currency = @Currency OR @Currency IS NULL
			)
		--	In the case when more than one currency was received.
		GROUP BY Currency
		)

	--  https://action.mindjet.com/task/14526658
	DECLARE @ID INT
	SET @ID = (
		SELECT ID
		FROM Month
		WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
	)

	IF @ID IS NOT NULL
	BEGIN
		--	Updates Income value.
		UPDATE Month
		SET Income = @TotalIncome
		WHERE Year = @Year AND Month = @Month 
			AND DataOwner = @DataOwner
	END
	ELSE
	BEGIN
		--	Inserts new Income value.
		INSERT INTO Month (Income, Year, Month, DataOwner)
		VALUES (@TotalIncome, @Year, @Month, @DataOwner)
	END
END
GO
