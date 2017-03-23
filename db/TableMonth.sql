
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 23/07/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
-- =============================================

ALTER TABLE Month ADD Currency NCHAR(5);
GO

/****** Object:  Table [dbo].[Month]    Script Date: 02.06.2015 21:43:37 ******/
/* https://www.evernote.com/shard/s132/nl/14501366/14e369f7-348f-4f68-aa65-6a5e7dda1da7 */

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Month]
GO

CREATE TABLE [dbo].[Month](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Budget] [money] NULL,
	[Income] [money] NULL,
	[DataOwner] [uniqueidentifier] NOT NULL,
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL
 CONSTRAINT [PK_Month] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 
GO

