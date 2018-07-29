CREATE TABLE [expenses].[ExpensesLinks] (
    [ExpenseID] INT    NOT NULL,
    [LinkID]    INT    NOT NULL,
    [ID]        BIGINT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ExpensesLinks] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ExpensesLinks_Expenses] FOREIGN KEY ([ExpenseID]) REFERENCES [expenses].[Operations] ([ID]),
    CONSTRAINT [FK_ExpensesLinks_Links] FOREIGN KEY ([LinkID]) REFERENCES [expenses].[Links] ([ID])
);

