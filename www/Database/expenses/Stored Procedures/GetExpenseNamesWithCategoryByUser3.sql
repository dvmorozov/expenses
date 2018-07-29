
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 30/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/8cc36717-9904-4f5c-bb2b-fffb8cfb37d1
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser3 
	@CategoryID INT,
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--	https://www.evernote.com/shard/s132/nl/14501366/adbb4c02-3975-460d-88f1-8a65312ca83f
	--	The last expense is selected in each group.
	SELECT e.Name, e.EncryptedName, c.Id, c.Count
	FROM [expenses].Expenses e
	JOIN 
	(
		--	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
		SELECT MAX(e.ID) AS Id, COUNT(*) AS Count
		FROM [expenses].Expenses e
		JOIN [expenses].ExpensesCategories ec
		ON e.ID = ec.ExpenseID
		WHERE CategoryID = @CategoryID AND DataOwner = @DataOwner
		GROUP BY NameChecksum
	) c
	ON c.Id = e.ID
	ORDER BY c.Count DESC
END
