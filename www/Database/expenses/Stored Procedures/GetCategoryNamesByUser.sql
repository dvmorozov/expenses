
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 18/01/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/42b4d734-28a0-48b6-9403-148faa8409a2
-- =============================================
CREATE PROCEDURE [expenses].GetCategoryNamesByUser
	@DataOwner UNIQUEIDENTIFIER 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ID, LTRIM(RTRIM(Name)) AS Name
	FROM [expenses].Categories
	WHERE DataOwner = @DataOwner
	ORDER BY Name
END
