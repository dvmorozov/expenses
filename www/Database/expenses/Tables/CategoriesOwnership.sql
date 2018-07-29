CREATE TABLE [expenses].[CategoriesOwnership] (
    [OwnerID] INT    NULL,
    [OwneeID] INT    NOT NULL,
    [ID]      BIGINT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_CategoriesOwnership] PRIMARY KEY CLUSTERED ([ID] ASC)
);

