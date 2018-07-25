
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 10/05/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/4bc52f46-5b79-4788-824d-f3a4b0e9fad3
-- =============================================

CREATE PROCEDURE [expenses].EstimatedTop10CategoriesForMonthByUser @Year int, @Month int, @Day INT, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	DECLARE @T TABLE (
		TOTAL FLOAT NOT NULL,
		LIMIT FLOAT NULL,
		NAME CHAR(100) NOT NULL, 
		ID INT NOT NULL, 
		ESTIMATION NVARCHAR(MAX) NOT NULL, 
		EncryptedName NVARCHAR(MAX) NULL,
		Currency NCHAR(5),
		GROUPID1 INT NOT NULL,
		GROUPID2 INT NOT NULL
		)

	INSERT INTO @T EXEC [expenses].EstimatedTop10CategoriesForMonthByUser3 @Year, @Month, @Day, @DataOwner

	SELECT TOTAL, LIMIT, NAME, ID, ESTIMATION
	FROM @T
END
