CREATE TABLE [expenses].[Languages] (
    [ID]       INT        IDENTITY (1, 1) NOT NULL,
    [Language] NCHAR (20) NOT NULL,
    CONSTRAINT [PK_Languages] PRIMARY KEY CLUSTERED ([ID] ASC)
);

