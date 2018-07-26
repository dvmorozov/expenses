
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 12/07/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
-- =============================================

CREATE PROCEDURE [expenses].TodayExpensesByUser3 @Today DATETIME, @DataOwner UNIQUEIDENTIFIER 
AS
	SELECT 
		e.Date, LTRIM(RTRIM(e.Name)) AS Name, e.Cost, LTRIM(RTRIM(e.Note)) AS Note, e.ID, LTRIM(RTRIM(c.Name)) AS CategoryName,
		e.EncryptedName AS ExpenseEncryptedName, c.EncryptedName AS CategoryEncryptedName, e.Currency
	FROM [expenses].Expenses e
	INNER JOIN ExpensesCategories ec
	ON ec.ExpenseID = e.ID
	INNER JOIN [expenses].Categories c
	ON ec.CategoryID = c.ID
	WHERE e.DataOwner = @DataOwner 
		--	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
		AND (
				(
					(e.Monthly IS NULL OR e.Monthly = 0) AND
					DATEPART(YEAR, DATE) = DATEPART(YEAR, @Today) AND 
					DATEPART(MONTH, DATE) = DATEPART(MONTH, @Today) AND
					DATEPART(DAY, DATE) = DATEPART(DAY, @Today)
				)
			OR
				(
					(e.Monthly IS NOT NULL AND e.Monthly = 1) AND
					DATEPART(DAY, Date) = DATEPART(DAY, @Today) AND
					(@Today >= e.FirstMonth) AND (@Today <= EOMONTH(e.LastMonth) OR LastMonth IS NULL)
				)
			)
	--	The last expenses first.
	ORDER BY e.ID DESC
