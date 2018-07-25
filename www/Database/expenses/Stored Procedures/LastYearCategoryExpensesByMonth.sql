
CREATE PROCEDURE [expenses].LastYearCategoryExpensesByMonth @CategoryID INT, @LastMonthNumber INT
AS
BEGIN
	DECLARE @DataOwner UNIQUEIDENTIFIER
	SET @DataOwner = 'D94F9786-01F3-4B22-B612-285F82A85093'

	EXEC LastYearCategoryExpensesByMonthByUser2 @CategoryID, @LastMonthNumber, @DataOwner
END
