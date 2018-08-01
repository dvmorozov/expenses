
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Create date: 07/12/2015
-- Description:	https://action.mindjet.com/task/14479694
-- ==========================================================================================

CREATE PROCEDURE [expenses].EstimatedCategoriesByUser3 @Year INT, @Month INT, @Day INT, @DataOwner UNIQUEIDENTIFIER, @ShortList BIT
AS 
BEGIN
	SELECT
		Name AS NAME,
		Limit,
		CategoryID AS ID,
		CategoryTotal AS Total,
		Estimation,
		EncryptedName
	FROM [expenses].GetEstimatedCategoriesByUser(@Year, @Month, @Day, @DataOwner, @ShortList)
END