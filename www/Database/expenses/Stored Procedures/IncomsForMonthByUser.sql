
CREATE PROCEDURE [expenses].[IncomsForMonthByUser] @Year int, @Month int, @DataOwner UNIQUEIDENTIFIER 
AS 
BEGIN
	SELECT ID, Date, Cost AS Amount, Name, EncryptedName
	FROM [expenses].Operations
	WHERE DataOwner = @DataOwner AND Income IS NOT NULL AND Income = 1 AND DATEPART(YEAR, DATE) = @YEAR AND DATEPART(MONTH, DATE) = @MONTH 
	ORDER BY Date ASC
END
