DROP PROCEDURE GetCategoryNamesByUser
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 18/01/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/42b4d734-28a0-48b6-9403-148faa8409a2
-- =============================================
CREATE PROCEDURE GetCategoryNamesByUser
	@DataOwner UNIQUEIDENTIFIER 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ID, LTRIM(RTRIM(Name)) AS Name
	FROM Categories
	WHERE DataOwner = @DataOwner
	ORDER BY Name
END
GO

DROP PROCEDURE GetCategoryNamesByUser2
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 19/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE GetCategoryNamesByUser2
	@DataOwner UNIQUEIDENTIFIER 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ID, LTRIM(RTRIM(Name)) AS Name, EncryptedName
	FROM Categories
	WHERE DataOwner = @DataOwner
	ORDER BY Name
END
GO
