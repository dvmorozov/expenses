
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 09/03/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/0fc35d9b-80d0-4159-be22-4d935bc2825a
-- =============================================
CREATE PROCEDURE [expenses].EstimatedCategoriesByUser @Year INT, @Month INT, @Day INT, @DataOwner UNIQUEIDENTIFIER 
AS 
	DECLARE @T TABLE (
		NAME CHAR(100) NOT NULL, 
		Limit FLOAT NULL, 
		ID INT NOT NULL, 
		Total FLOAT NULL,
		Estimation NCHAR(20) NOT NULL,
		EncryptedName NVARCHAR(MAX) NULL
		)

	INSERT INTO @T EXEC [expenses].EstimatedCategoriesByUser2 @Year, @Month, @Day, @DataOwner 
	
	SELECT NAME, Limit, ID, Total, Estimation
	FROM @T
