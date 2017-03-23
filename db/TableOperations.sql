-- =============================================
-- Author:		D.V.Morozov
-- Create date: 12/12/2015
-- Description:	https://vision.mindjet.com/action/task/14485573
-- =============================================

ALTER TABLE Operations ADD Income BIT;
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 18/12/2015
-- Description:	https://vision.mindjet.com/action/task/14485573
-- =============================================

IF OBJECT_ID ('TG_UpdateNameChecksum','TR') IS NOT NULL
    DROP TRIGGER TG_UpdateNameChecksum;
GO

CREATE TRIGGER TG_UpdateNameChecksum
ON dbo.Operations
AFTER INSERT, UPDATE
AS
   UPDATE Operations
   SET NameChecksum = CHECKSUM(i.Name, i.EncryptedName)
   FROM Operations e
   JOIN (SELECT * FROM inserted) i
   ON i.ID = e.ID
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 18/12/2015
-- Description:	https://vision.mindjet.com/action/task/14485573
-- =============================================

IF OBJECT_ID ('TG_UpdateMonthIncome','TR') IS NOT NULL
    DROP TRIGGER TG_UpdateMonthIncome;
GO

CREATE TRIGGER TG_UpdateMonthIncome
ON dbo.Operations
AFTER INSERT, UPDATE
AS
	DECLARE @t TABLE
	(
		Year INT NOT NULL,
		Month INT NOT NULL,
		DataOwner UNIQUEIDENTIFIER
	)

	INSERT INTO @t
    SELECT DISTINCT DATEPART(year, Date) Year, DATEPART(month, Date) Month, DataOwner 
	FROM inserted

	DECLARE @cursor CURSOR, @Year INT, @Month INT, @DataOwner UNIQUEIDENTIFIER
	SET @cursor = CURSOR FOR 
		SELECT Year, Month, DataOwner FROM @t
	OPEN @cursor
		
	WHILE 1=1
	BEGIN
		FETCH FROM @cursor INTO @Year, @Month, @DataOwner
		IF @@fetch_status <> 0 BREAK

		EXEC RecalcMonthIncome @Year, @Month, @DataOwner
	END
GO

IF OBJECT_ID ('TG_DeleteMonthIncome','TR') IS NOT NULL
    DROP TRIGGER TG_DeleteMonthIncome;
GO

CREATE TRIGGER TG_DeleteMonthIncome
ON dbo.Operations
AFTER DELETE
AS
	DECLARE @t TABLE
	(
		Year INT NOT NULL,
		Month INT NOT NULL,
		DataOwner UNIQUEIDENTIFIER
	)

	INSERT INTO @t
    SELECT DISTINCT DATEPART(year, Date) Year, DATEPART(month, Date) Month, DataOwner 
	FROM deleted

	DECLARE @cursor CURSOR, @Year INT, @Month INT, @DataOwner UNIQUEIDENTIFIER
	SET @cursor = CURSOR FOR 
		SELECT Year, Month, DataOwner FROM @t
	OPEN @cursor
		
	WHILE 1=1
	BEGIN
		FETCH FROM @cursor INTO @Year, @Month, @DataOwner
		IF @@fetch_status <> 0 BREAK

		EXEC RecalcMonthIncome @Year, @Month, @DataOwner
	END
GO
