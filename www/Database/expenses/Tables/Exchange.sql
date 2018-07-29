CREATE TABLE [expenses].[Exchange] (
    [EUR]  FLOAT (53) NULL,
    [USD]  FLOAT (53) NULL,
    [Date] DATETIME   NOT NULL,
    [ID]   BIGINT     IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Exchange] PRIMARY KEY CLUSTERED ([ID] ASC)
);

