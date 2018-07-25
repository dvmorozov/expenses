-- =============================================
-- Author:		D.V.Morozov
-- Create date: 11/01/2014
-- Description:	evernote:///view/14501366/s132/ef3044c2-e6f9-47c4-8627-6a3199e39db2/ef3044c2-e6f9-47c4-8627-6a3199e39db2/
-- =============================================
CREATE PROCEDURE [expenses].GetExpenseNames 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Name, ID
	FROM [expenses].ExpenseNames
	ORDER BY Name
END
