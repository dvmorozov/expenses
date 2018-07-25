-- =============================================
-- Author:		D.V.Morozov
-- Create date: 16/12/2015
-- Description:	https://vision.mindjet.com/action/task/14485574
-- =============================================
CREATE PROCEDURE [expenses].DeleteOperationByUser 
	@ExpenseId INT,
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @DO UNIQUEIDENTIFIER

	SET @DO = (SELECT TOP(1) @DataOwner FROM Operations WHERE ID = @ExpenseId)

	IF @DO = @DataOwner
	BEGIN
		BEGIN TRANSACTION

		DELETE FROM [expenses].ExpensesCategories
		WHERE ExpenseId = @ExpenseId

		--	https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
		DELETE FROM [expenses].ExpensesLinks
		WHERE ExpenseId = @ExpenseId

		DELETE FROM [expenses].Operations
		WHERE ID = @ExpenseId

		COMMIT
	END
END

