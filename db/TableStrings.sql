USE [ExpensesDev]
GO

/****** Object:  Table [dbo].[Strings]    Script Date: 01/15/2014 19:24:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Strings](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LangId] [int] NOT NULL,
	[ControlId] [int] NOT NULL,
	[Text] [nvarchar](500) NULL,
 CONSTRAINT [PK_Strings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'NULL means that the default text (in English) will be used.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Strings', @level2type=N'COLUMN',@level2name=N'Text'
GO

ALTER TABLE [dbo].[Strings]  WITH CHECK ADD  CONSTRAINT [FK_Strings_Controls] FOREIGN KEY([ControlId])
REFERENCES [dbo].[Controls] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Strings] CHECK CONSTRAINT [FK_Strings_Controls]
GO

ALTER TABLE [dbo].[Strings]  WITH CHECK ADD  CONSTRAINT [FK_Strings_Languages] FOREIGN KEY([LangId])
REFERENCES [dbo].[Languages] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Strings] CHECK CONSTRAINT [FK_Strings_Languages]
GO


