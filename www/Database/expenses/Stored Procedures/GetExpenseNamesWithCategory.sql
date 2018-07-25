-- =============================================
-- Author:		D.V.Morozov
-- Create date: 11/01/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/e34ce591-b9dd-449b-b618-84208ec62585
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategory 
	@CategoryID INT
AS
BEGIN
	DECLARE @T TABLE (
		Name NCHAR(50) NOT NULL, 
		Id INT NOT NULL,
		EncryptedName NVARCHAR(MAX) NULL
		)

	--	https://www.evernote.com/shard/s132/nl/14501366/a121f3e5-a6e7-4523-ab86-2b6868a773b8
	INSERT INTO @T EXEC GetExpenseNamesWithCategoryByUser2 @CategoryID, 'D94F9786-01F3-4B22-B612-285F82A85093'
	
	SELECT Name, Id
	FROM @T
END
