-- =============================================
-- Author:		D.V.Morozov
-- Create date: 12/07/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
-- =============================================
CREATE PROCEDURE [expenses].AddExpenseByUser4
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

	DECLARE @ExpenseIDs TABLE (ID INT)
	
	BEGIN TRAN
	
	INSERT INTO [expenses].Expenses (Date, Name, Cost, Note, DataOwner, Monthly, FirstMonth, LastMonth, EncryptedName, Currency)
	OUTPUT INSERTED.ID INTO @ExpenseIDs
	VALUES (@Date, @Name, @Cost, @Note, @DataOwner, @Monthly, @FirstMonth, @LastMonth, @EncryptedName, @Currency)
	
	DECLARE @ExpenseID INT
	SET @ExpenseID = (SELECT TOP(1) ID FROM @ExpenseIDs)
	
	INSERT INTO [expenses].ExpensesCategories (ExpenseID, CategoryID)
	VALUES (@ExpenseID, @CategoryID)
	
	COMMIT

	SELECT @ExpenseID
END
