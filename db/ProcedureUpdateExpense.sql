
DROP PROCEDURE [expenses].UpdateExpenseByUser
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 22/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================
CREATE PROCEDURE [expenses].UpdateExpenseByUser 
	@ExpenseID INT,
	@Name NCHAR(50),
	@EncryptedName NVARCHAR(MAX),
	@DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @N NCHAR(50)
	SET @N = (SELECT LTRIM(RTRIM(Name)) 
	FROM [expenses].Expenses WHERE ID = @ExpenseID AND DataOwner = @DataOwner AND EncryptedName IS NULL)

	--	Updates only one time.
	IF @N IS NOT NULL AND @N <> ''
	BEGIN
		UPDATE [expenses].Expenses
		SET Name = @Name, EncryptedName = @EncryptedName
		WHERE Name = @N AND DataOwner = @DataOwner
	END
END
GO

DROP PROCEDURE [expenses].UpdateExpenseByUser2
GO

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
GO


