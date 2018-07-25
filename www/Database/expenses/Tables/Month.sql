CREATE TABLE [expenses].[Month] (
    [ID]        INT              IDENTITY (1, 1) NOT NULL,
    [Budget]    MONEY            NULL,
    [Income]    MONEY            NULL,
    [DataOwner] UNIQUEIDENTIFIER NOT NULL,
    [Year]      INT              NOT NULL,
    [Month]     INT              NOT NULL,
    [Currency]  NCHAR (5)        NULL,
    CONSTRAINT [PK_Month] PRIMARY KEY CLUSTERED ([ID] ASC)
);

