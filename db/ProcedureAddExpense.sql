
DROP PROCEDURE [expenses].AddExpense
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO

DROP PROCEDURE [expenses].AddExpenseByUser
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO

DROP PROCEDURE [expenses].AddExpenseByUser2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
GO

DROP PROCEDURE [expenses].AddExpenseByUser3
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
GO

DROP PROCEDURE [expenses].AddExpenseByUser4
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
