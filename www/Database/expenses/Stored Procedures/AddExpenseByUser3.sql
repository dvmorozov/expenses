
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 18/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE [expenses].AddExpenseByUser3
	@Date DATETIME, 
	@Name NCHAR(50),
	@Cost FLOAT,
	@Note NCHAR(200),
	@CategoryID	INT,
	@DataOwner UNIQUEIDENTIFIER,
	@Monthly BIT,
	@FirstMonth DATE,
	@LastMonth DATE,
	@EncryptedName NVARCHAR(MAX) = NULL
AS
BEGIN
	EXEC [expenses].AddExpenseByUser4 @Date, @Name, @Cost,	@Note, @CategoryID,	@DataOwner,	@Monthly, @FirstMonth, @LastMonth, @EncryptedName, NULL
END
