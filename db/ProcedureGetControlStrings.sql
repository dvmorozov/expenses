
DROP PROCEDURE [expenses].GetControlStrings
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 08/01/2013
-- Description:	evernote:///view/14501366/s132/04464e18-8439-4182-8b5a-bf3171762cc4/04464e18-8439-4182-8b5a-bf3171762cc4/
-- =============================================
CREATE PROCEDURE [expenses].GetControlStrings 
	@LangId INT = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT RTRIM(ControlName) AS ControlName, RTRIM(Text) AS Text
	FROM [expenses].ControlStrings
	WHERE LanguageId = @LangId
END
GO
