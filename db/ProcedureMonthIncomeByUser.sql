
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DROP PROCEDURE [expenses].MonthIncomeByUser
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 06/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/e9be060a-5343-47e7-9441-65cbb5c80f60
-- =============================================
CREATE PROCEDURE [expenses].MonthIncomeByUser @Today DATETIME, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @Year INT
	SET @Year = DATEPART(YEAR, @Today)

	DECLARE @Month INT
	SET @Month = DATEPART(MONTH, @Today)

	SELECT Income
	FROM [expenses].Month
	WHERE Year = @Year AND Month = @Month AND DataOwner = @DataOwner
END
GO
