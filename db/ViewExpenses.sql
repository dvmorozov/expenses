
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 12/12/2015
-- Description:	https://vision.mindjet.com/action/task/14485573
--				https://vision.mindjet.com/action/task/14485574
-- =============================================

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP VIEW [expenses].[Expenses]
GO

CREATE VIEW [expenses].[Expenses]
AS
SELECT ID, Date, Cost, LastUpdate, Note, DataOwner, Monthly, FirstMonth, LastMonth, Currency, Rating, Importance, EncryptedName, NameChecksum, Project, Name
FROM     expenses.Operations
WHERE  (Income IS NULL) OR (Income = 0)

GO
