
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 21/05/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE [expenses].EstimatedTop10CategoriesForMonthByUser2 @Year int, @Month int, @Day INT, @DataOwner UNIQUEIDENTIFIER 
AS 
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

	SELECT TOTAL, LIMIT, NAME, ID, ESTIMATION, EncryptedName
	FROM @T
