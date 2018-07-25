CREATE TABLE [expenses].[Strings] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [LangId]    INT            NOT NULL,
    [ControlId] INT            NOT NULL,
    [Text]      NVARCHAR (500) NULL,
    CONSTRAINT [PK_Strings] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Strings_Controls] FOREIGN KEY ([ControlId]) REFERENCES [expenses].[Controls] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Strings_Languages] FOREIGN KEY ([LangId]) REFERENCES [expenses].[Languages] ([ID]) ON DELETE CASCADE
);

