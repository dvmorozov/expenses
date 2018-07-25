
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 12/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
-- Returns:		Expense id.
-- =============================================
CREATE PROCEDURE [expenses].AddExpenseByUser2
	@Date DATETIME, 
	@Name NCHAR(50),
	@Cost FLOAT,
	@Note NCHAR(200),
	@CategoryID	INT,
	@DataOwner UNIQUEIDENTIFIER,
	@Monthly BIT,
	@FirstMonth DATE,
	@LastMonth DATE
AS
BEGIN
	EXEC [expenses].AddExpenseByUser3 @Date, @Name, @Cost,	@Note, @CategoryID,	@DataOwner,	@Monthly, @FirstMonth, @LastMonth, NULL
END
