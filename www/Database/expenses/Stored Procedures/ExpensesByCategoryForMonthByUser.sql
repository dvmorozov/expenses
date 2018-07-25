
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 22/03/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/751d5935-68c5-42be-8f12-c5ab2315da02
-- =============================================
CREATE PROCEDURE [expenses].ExpensesByCategoryForMonthByUser @CategoryId int, @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	DECLARE @T TABLE (
		NAME NCHAR(50) NOT NULL, 
		TOTAL FLOAT NOT NULL, 
		EncryptedName NVARCHAR(MAX) NULL
		)

	INSERT INTO @T EXEC [expenses].ExpensesByCategoryForMonthByUser2 @CategoryId, @Year, @Month, @DataOwner

	SELECT NAME, TOTAL
	FROM @T
END
