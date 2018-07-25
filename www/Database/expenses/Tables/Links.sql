CREATE TABLE [expenses].[Links] (
    [ID]        INT              IDENTITY (1, 1) NOT NULL,
    [URL]       NVARCHAR (MAX)   NOT NULL,
    [Name]      NVARCHAR (MAX)   NOT NULL,
    [DataOwner] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Links] PRIMARY KEY CLUSTERED ([ID] ASC)
);

