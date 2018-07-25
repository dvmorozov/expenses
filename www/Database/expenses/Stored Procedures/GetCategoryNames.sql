-- =============================================
-- Author:		D.V.Morozov
-- Create date: 14/01/2014
-- Description:	evernote:///view/14501366/s132/ef3044c2-e6f9-47c4-8627-6a3199e39db2/ef3044c2-e6f9-47c4-8627-6a3199e39db2/
-- =============================================
CREATE PROCEDURE [expenses].GetCategoryNames 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ID, LTRIM(RTRIM(Name)) AS Name
	FROM Categories
	--	https://www.evernote.com/shard/s132/nl/14501366/a121f3e5-a6e7-4523-ab86-2b6868a773b8
	WHERE DataOwner = 'D94F9786-01F3-4B22-B612-285F82A85093'
	ORDER BY Name
END
