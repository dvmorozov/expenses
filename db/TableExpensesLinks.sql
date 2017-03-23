
--DROP TABLE [dbo].[ExpensesLinks]
--GO

/****** Object:  Table [dbo].[ExpensesLinks]    Script Date: 23.09.2015 21:09:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExpensesLinks](
	[ExpenseID] [int] NOT NULL,
	[LinkID] [int] NOT NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_ExpensesLinks] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 

GO

ALTER TABLE [dbo].[ExpensesLinks]  WITH CHECK ADD  CONSTRAINT [FK_ExpensesLinks_Expenses] FOREIGN KEY([ExpenseID])
REFERENCES [dbo].[Expenses] ([ID])
GO

ALTER TABLE [dbo].[ExpensesLinks] CHECK CONSTRAINT [FK_ExpensesLinks_Expenses]
GO

ALTER TABLE [dbo].[ExpensesLinks]  WITH CHECK ADD  CONSTRAINT [FK_ExpensesLinks_Links] FOREIGN KEY([LinkID])
REFERENCES [dbo].[Links] ([ID])
GO

ALTER TABLE [dbo].[ExpensesLinks] CHECK CONSTRAINT [FK_ExpensesLinks_Links]
GO


