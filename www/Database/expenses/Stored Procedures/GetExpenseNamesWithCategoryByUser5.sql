
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 09/12/2015
-- Description:	https://action.mindjet.com/task/14479694
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser5
	@CategoryID INT,
	@DataOwner UNIQUEIDENTIFIER,
	@Year INT, @Month INT, @Day INT,
	@ShortList BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--	Date to start from.
	DECLARE @DateFrom DATETIME
	--	Selects for last half a year.
	SET @DateFrom = DATEADD(month, -6, GETDATE());

	--	https://www.evernote.com/shard/s132/nl/14501366/adbb4c02-3975-460d-88f1-8a65312ca83f
	--	The last expense is selected in each group.
	SELECT e.Name, e.EncryptedName, c.Id
	FROM [expenses].Expenses e
	JOIN 
	(
		--	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
		SELECT MAX(e.ID) AS Id
		FROM [expenses].Expenses e
		JOIN [expenses].ExpensesCategories ec
		ON e.ID = ec.ExpenseID
		WHERE CategoryID = @CategoryID AND DataOwner = @DataOwner
			AND
			(
				@ShortList = 0 OR 
				(Date > @DateFrom OR (Monthly IS NOT NULL AND Monthly = 1 AND (LastMonth IS NULL OR LastMonth > @DateFrom)))
			)
		GROUP BY NameChecksum
	) c
	ON c.Id = e.ID
END
