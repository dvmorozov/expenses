/****** Object:  View [expenses].[Chronological]    Script Date: 4/30/2018 10:24:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [expenses].[Chronological]
AS
SELECT TOP (100) PERCENT ID, Date, Cost, LastUpdate, Note, DataOwner, Monthly, FirstMonth, LastMonth, Currency, Rating, Importance, EncryptedName, NameChecksum, Project, 
                  Name
FROM     expenses.Expenses
ORDER BY Date
GO


