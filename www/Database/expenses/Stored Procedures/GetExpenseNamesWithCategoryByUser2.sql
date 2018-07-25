
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 19/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/d8e9d2dc-b1df-47af-882e-f84727e5c435
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNamesWithCategoryByUser2 
	@CategoryID INT,
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @T TABLE (
		Name NCHAR(50) NOT NULL, 
		Id INT NOT NULL,
		EncryptedName NVARCHAR(MAX) NULL,
		Count INT NOT NULL
		)

	INSERT INTO @T EXEC [expenses].GetExpenseNamesWithCategoryByUser3 @CategoryID, @DataOwner 
	
	SELECT Name, Id, EncryptedName
	FROM @T
END
