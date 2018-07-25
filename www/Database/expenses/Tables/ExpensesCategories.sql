CREATE TABLE [expenses].[ExpensesCategories] (
    [ExpenseID]  INT    NOT NULL,
    [CategoryID] INT    NOT NULL,
    [ID]         BIGINT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ExpensesCategories] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ExpensesCategories_Categories] FOREIGN KEY ([CategoryID]) REFERENCES [expenses].[Categories] ([ID]),
    CONSTRAINT [FK_ExpensesCategories_Expenses] FOREIGN KEY ([ExpenseID]) REFERENCES [expenses].[Operations] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_ExpensesCategories_CategoryID]
    ON [expenses].[ExpensesCategories]([CategoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExpensesCategories_ExpenseID]
    ON [expenses].[ExpensesCategories]([ExpenseID] ASC);

