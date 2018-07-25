
CREATE PROCEDURE [expenses].Top10CategoriesForMonthByUser @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS
BEGIN
	DECLARE @T TABLE (
		NAME CHAR(100) NOT NULL, 
		TOTAL FLOAT NOT NULL,
		ID INT NOT NULL, 
		EncryptedName NVARCHAR(MAX) NULL
		)

	INSERT INTO @T EXEC [expenses].Top10CategoriesForMonthByUser2 @Year, @Month, @DataOwner

	SELECT NAME, TOTAL, ID
	FROM @T
END
