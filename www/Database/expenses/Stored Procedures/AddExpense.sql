-- =============================================
-- Author:		D.V.Morozov
-- Create date: 11/01/2011
-- Description:	evernote:///view/14501366/s132/ef3044c2-e6f9-47c4-8627-6a3199e39db2/ef3044c2-e6f9-47c4-8627-6a3199e39db2/
-- =============================================
CREATE PROCEDURE [expenses].AddExpense 
	@Date DATETIME, 
	@Name NCHAR(50),
	@Cost FLOAT,
	@Note NCHAR(200),
	@CategoryID	INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ExpenseIDs TABLE (ID INT)
	
	BEGIN TRAN
	
	INSERT INTO [expenses].Expenses (Date, Name, Cost, Note)
	OUTPUT INSERTED.ID INTO @ExpenseIDs
	VALUES (@Date, @Name, @Cost, @Note)
	
	DECLARE @ExpenseID INT
	SET @ExpenseID = (SELECT TOP(1) ID FROM @ExpenseIDs)
	
	INSERT INTO [expenses].ExpensesCategories (ExpenseID, CategoryID)
	VALUES (@ExpenseID, @CategoryID)
	
	COMMIT
END
