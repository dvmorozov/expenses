-- =============================================
-- Author:		D.V.Morozov
-- Create date: 19/01/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/42b4d734-28a0-48b6-9403-148faa8409a2
-- =============================================
CREATE PROCEDURE [expenses].AddExpenseByUser 
	@Date DATETIME, 
	@Name NCHAR(50),
	@Cost FLOAT,
	@Note NCHAR(200),
	@CategoryID	INT,
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ExpenseIDs TABLE (ID INT)
	
	BEGIN TRAN
	
	INSERT INTO [expenses].Expenses (Date, Name, Cost, Note, DataOwner)
	OUTPUT INSERTED.ID INTO @ExpenseIDs
	VALUES (@Date, @Name, @Cost, @Note, @DataOwner)
	
	DECLARE @ExpenseID INT
	SET @ExpenseID = (SELECT TOP(1) ID FROM @ExpenseIDs)
	
	INSERT INTO [expenses].ExpensesCategories (ExpenseID, CategoryID)
	VALUES (@ExpenseID, @CategoryID)
	
	COMMIT
END
