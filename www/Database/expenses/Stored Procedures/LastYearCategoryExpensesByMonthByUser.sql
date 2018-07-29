
CREATE PROCEDURE [expenses].LastYearCategoryExpensesByMonthByUser @CategoryID INT, @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @T TABLE (
		Y INT NOT NULL, 
		M INT NOT NULL,
		Total FLOAT NOT NULL, 
		Month NVARCHAR(10) NULL
		)

	INSERT INTO @T EXEC [expenses].LastYearCategoryExpensesByMonthByUser2 @CategoryID, @LastMonthNumber, @DataOwner

	SELECT Y, M, Total
	FROM @T
END
