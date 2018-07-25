-- =============================================
-- Author:		D.V.Morozov
-- Create date: 25/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE [expenses].UpdateCategoryByUser 
	@CategoryID INT,
	@Name CHAR(100),
	@EncryptedName NVARCHAR(MAX),
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE [expenses].Categories
	SET Name = @Name, EncryptedName = @EncryptedName
	WHERE ID = @CategoryID AND DataOwner = @DataOwner
END
