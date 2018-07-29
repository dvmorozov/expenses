CREATE TABLE [expenses].[Contacts] (
    [ContactId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (MAX) NULL,
    [Address]   NVARCHAR (MAX) NULL,
    [City]      NVARCHAR (MAX) NULL,
    [State]     NVARCHAR (MAX) NULL,
    [Zip]       NVARCHAR (MAX) NULL,
    [Email]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Contacts] PRIMARY KEY CLUSTERED ([ContactId] ASC)
);

