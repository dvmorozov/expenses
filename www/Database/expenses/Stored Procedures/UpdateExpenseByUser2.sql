
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 09/09/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/adbb4c02-3975-460d-88f1-8a65312ca83f
-- =============================================
CREATE PROCEDURE [expenses].UpdateExpenseByUser2
	@ExpenseID INT,
	@Date DATETIME, 
	@Name NCHAR(50),
	@Cost FLOAT,
	@Note NCHAR(200),
	@CategoryID	INT,
	@DataOwner UNIQUEIDENTIFIER,
	@Monthly BIT,
	@FirstMonth DATE,
	@LastMonth DATE,
	@EncryptedName NVARCHAR(MAX) = NULL,
	@Currency NCHAR(5) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [expenses].Expenses 
	SET Date = @Date, Name = @Name, Cost = @Cost, Note = @Note, Monthly = @Monthly, FirstMonth = @FirstMonth, LastMonth = @LastMonth, EncryptedName = @EncryptedName, Currency = @Currency
	WHERE ID = @ExpenseID AND DataOwner = @DataOwner
END
