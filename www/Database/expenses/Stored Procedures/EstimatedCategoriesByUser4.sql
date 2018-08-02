
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Create date: 02/08/2018
-- Description:	https://github.com/dvmorozov/expenses/issues/38
-- ==========================================================================================

CREATE PROCEDURE [expenses].EstimatedCategoriesByUser4 @Year INT, @Month INT, @Day INT, @DataOwner UNIQUEIDENTIFIER, @ShortList BIT
AS 
BEGIN
	SELECT
		Name AS NAME,
		Limit,
		CategoryID AS ID,
		CategoryTotal AS Total,
		Estimation,
		EncryptedName,
		Currency
	FROM [expenses].GetEstimatedCategoriesByUser(@Year, @Month, @Day, @DataOwner, @ShortList)
END