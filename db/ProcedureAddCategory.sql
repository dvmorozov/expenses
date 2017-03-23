
DROP PROCEDURE AddCategoryByUser
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 19/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE AddCategoryByUser
	@Name NCHAR(100),
	@Limit FLOAT,
	@DataOwner UNIQUEIDENTIFIER,
	@EncryptedName NVARCHAR(MAX) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Checks if the category with such name already exists.
	DECLARE @CategoryID INT
	--	https://www.evernote.com/shard/s132/nl/14501366/25896f00-ff68-4e71-af5e-736cee72fa43
	IF @EncryptedName IS NOT NULL
	BEGIN
		SET @CategoryID = (SELECT TOP(1) ID FROM Categories WHERE EncryptedName = @EncryptedName AND DataOwner = @DataOwner)
	END
	ELSE
	BEGIN
		SET @CategoryID = (SELECT TOP(1) ID FROM Categories WHERE Name = @Name AND DataOwner = @DataOwner)
	END

	IF @CategoryID IS NULL
	BEGIN
		DECLARE @CategoryIDs TABLE (ID INT)
	
		INSERT INTO Categories (Name, Limit, DataOwner, EncryptedName)
		OUTPUT INSERTED.ID INTO @CategoryIDs
		VALUES (@Name, @Limit, @DataOwner, @EncryptedName)
	
		SET @CategoryID = (SELECT TOP(1) ID FROM @CategoryIDs)
	END

	SELECT @CategoryID
END
GO
