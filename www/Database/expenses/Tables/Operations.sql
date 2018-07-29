CREATE TABLE [expenses].[Operations] (
    [ID]            INT              IDENTITY (1, 1) NOT NULL,
    [Date]          DATETIME         NOT NULL,
    [Cost]          FLOAT (53)       NULL,
    [LastUpdate]    ROWVERSION       NOT NULL,
    [Note]          CHAR (200)       NULL,
    [DataOwner]     UNIQUEIDENTIFIER CONSTRAINT [DataOwnerExpenseDefault] DEFAULT ('D94F9786-01F3-4B22-B612-285F82A85093') NOT NULL,
    [Monthly]       BIT              NULL,
    [FirstMonth]    DATE             NULL,
    [LastMonth]     DATE             NULL,
    [Currency]      NCHAR (5)        NULL,
    [Rating]        SMALLINT         NULL,
    [Importance]    SMALLINT         NULL,
    [EncryptedName] NVARCHAR (MAX)   NULL,
    [NameChecksum]  INT              NULL,
    [Project]       NVARCHAR (MAX)   NULL,
    [Name]          NVARCHAR (MAX)   DEFAULT ('') NOT NULL,
    [Income]        BIT              NULL,
    CONSTRAINT [PK_Expenses] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Expenses_NameChecksum]
    ON [expenses].[Operations]([NameChecksum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Expenses_DataOwner]
    ON [expenses].[Operations]([DataOwner] ASC);


GO
CREATE NONCLUSTERED INDEX [nci_wi_Operations_1FB38799733D81C7017A9D30B9EA3B72]
    ON [expenses].[Operations]([DataOwner] ASC, [Monthly] ASC, [FirstMonth] ASC)
    INCLUDE([Cost], [Currency], [Date], [Income], [LastMonth]);


GO
CREATE NONCLUSTERED INDEX [IX_Expenses_DataOwner_Income]
    ON [expenses].[Operations]([DataOwner] ASC, [Income] ASC)
    INCLUDE([Currency]);


GO
CREATE NONCLUSTERED INDEX [IX_Expenses_DataOwner_Currency_Monthly_Income]
    ON [expenses].[Operations]([DataOwner] ASC, [Currency] ASC, [Monthly] ASC, [Income] ASC)
    INCLUDE([Date], [Cost]);


GO

CREATE TRIGGER [expenses].[TG_UpdateNameChecksum]
ON [expenses].[Operations]
AFTER INSERT, UPDATE
AS
   UPDATE expenses.Operations
   SET NameChecksum = CHECKSUM(i.Name, i.EncryptedName)
   FROM expenses.Operations e
   JOIN (SELECT * FROM inserted) i
   ON i.ID = e.ID
   

GO

CREATE TRIGGER [expenses].[TG_UpdateMonthIncome]
ON [expenses].[Operations]
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

		EXEC expenses.RecalcMonthIncome @Year, @Month, @DataOwner
	END

GO

CREATE TRIGGER [expenses].[TG_DeleteMonthIncome]
ON [expenses].[Operations]
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

		EXEC expenses.RecalcMonthIncome @Year, @Month, @DataOwner
	END
