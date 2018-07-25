-- =============================================
-- Author:		D.V.Morozov
-- Create date: 22/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE [expenses].UpdateExpenseByUser 
	@ExpenseID INT,
	@Name NCHAR(50),
	@EncryptedName NVARCHAR(MAX),
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @N NCHAR(50)
	SET @N = (SELECT LTRIM(RTRIM(Name)) 
	FROM [expenses].Expenses WHERE ID = @ExpenseID AND DataOwner = @DataOwner AND EncryptedName IS NULL)

	--	Updates only one time.
	IF @N IS NOT NULL AND @N <> ''
	BEGIN
		UPDATE [expenses].Expenses
		SET Name = @Name, EncryptedName = @EncryptedName
		WHERE Name = @N AND DataOwner = @DataOwner
	END
END
