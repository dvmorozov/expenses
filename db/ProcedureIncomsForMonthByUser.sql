
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		D.V.Morozov
-- Create date: 13/12/2015
-- Description:	https://vision.mindjet.com/action/task/14485575
-- =============================================

DROP PROCEDURE [expenses].[IncomsForMonthByUser]
GO

CREATE PROCEDURE [expenses].[IncomsForMonthByUser] @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	SELECT ID, Date, Cost AS Amount, Name, EncryptedName
	FROM [expenses].Operations
	WHERE DataOwner = @DataOwner AND Income IS NOT NULL AND Income = 1 AND DATEPART(YEAR, DATE) = @YEAR AND DATEPART(MONTH, DATE) = @MONTH 
	ORDER BY Date ASC
END
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 21/01/2017
-- Description:	https://action.mindjet.com/task/14834466
-- =============================================

DROP PROCEDURE [expenses].[IncomsForMonthByUser2]
GO

CREATE PROCEDURE [expenses].[IncomsForMonthByUser2] @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	SELECT ID, Date, Cost AS Amount, Name, EncryptedName, Currency
	FROM [expenses].Operations
	WHERE DataOwner = @DataOwner AND Income IS NOT NULL AND Income = 1 AND DATEPART(YEAR, DATE) = @YEAR AND DATEPART(MONTH, DATE) = @MONTH 
	ORDER BY Date ASC
END
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 06/04/2017
-- Description:	https://action.mindjet.com/task/14665340
-- =============================================

DROP PROCEDURE [expenses].[IncomsForMonthByUser3]
GO

CREATE PROCEDURE [expenses].[IncomsForMonthByUser3] @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	SELECT ID, Date, Cost AS Amount, Name, EncryptedName, Currency, Note
	FROM [expenses].Operations
	WHERE DataOwner = @DataOwner AND Income IS NOT NULL AND Income = 1 AND DATEPART(YEAR, DATE) = @YEAR AND DATEPART(MONTH, DATE) = @MONTH 
	ORDER BY Date ASC
END
GO

