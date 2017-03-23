
DROP PROCEDURE GetExpenseNames
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 11/01/2014
-- Description:	evernote:///view/14501366/s132/ef3044c2-e6f9-47c4-8627-6a3199e39db2/ef3044c2-e6f9-47c4-8627-6a3199e39db2/
-- =============================================
CREATE PROCEDURE GetExpenseNames 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Name, ID
	FROM ExpenseNames
	ORDER BY Name
END
GO

DROP PROCEDURE GetExpenseNamesByUser
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 19/01/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/42b4d734-28a0-48b6-9403-148faa8409a2
-- =============================================
CREATE PROCEDURE GetExpenseNamesByUser 
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Name, ID
	FROM ExpenseNames
	WHERE DataOwner = @DataOwner
	ORDER BY Name
END
GO
