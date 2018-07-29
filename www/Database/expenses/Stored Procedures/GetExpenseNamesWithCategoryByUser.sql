
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 19/01/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/42b4d734-28a0-48b6-9403-148faa8409a2
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser 
	@CategoryID INT,
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @T TABLE (
		Name NCHAR(50) NOT NULL, 
		Id INT NOT NULL,
		EncryptedName NVARCHAR(MAX) NULL
		)

	INSERT INTO @T EXEC [expenses].GetExpenseNamesWithCategoryByUser2 @CategoryID, @DataOwner 
	
	SELECT Name, Id
	FROM @T
END
