CREATE TABLE [expenses].[Categories] (
    [ID]            INT              IDENTITY (1, 1) NOT NULL,
    [Name]          CHAR (100)       NOT NULL,
    [DataOwner]     UNIQUEIDENTIFIER CONSTRAINT [DataOwnerCategoryDefault] DEFAULT ('D94F9786-01F3-4B22-B612-285F82A85093') NOT NULL,
    [Limit]         FLOAT (53)       NULL,
    [EncryptedName] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Idx_Categories]
    ON [expenses].[Categories]([DataOwner] ASC, [Name] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Categories_DataOwner]
    ON [expenses].[Categories]([DataOwner] ASC);

