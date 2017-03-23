USE [ExpensesDev]
GO

/****** Object:  Table [dbo].[ExpensesCategories]    Script Date: 04.11.2015 10:23:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExpensesCategories](
	[ExpenseID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_ExpensesCategories] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ExpensesCategories]  WITH CHECK ADD  CONSTRAINT [FK_ExpensesCategories_Categories] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([ID])
GO

ALTER TABLE [dbo].[ExpensesCategories] CHECK CONSTRAINT [FK_ExpensesCategories_Categories]
GO

ALTER TABLE [dbo].[ExpensesCategories]  WITH NOCHECK ADD  CONSTRAINT [FK_ExpensesCategories_Expenses] FOREIGN KEY([ExpenseID])
REFERENCES [dbo].[Expenses] ([ID])
GO

ALTER TABLE [dbo].[ExpensesCategories] CHECK CONSTRAINT [FK_ExpensesCategories_Expenses]
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 04/11/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
-- =============================================

CREATE NONCLUSTERED INDEX [IX_ExpensesCategories_ExpenseID] ON [dbo].[ExpensesCategories]
(
	[ExpenseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ExpensesCategories_CategoryID] ON [dbo].[ExpensesCategories]
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


