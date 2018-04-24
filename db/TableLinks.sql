
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 26/09/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
-- =============================================

ALTER TABLE [expenses].Links
ADD DataOwner UNIQUEIDENTIFIER NOT NULL
GO

/****** Object:  Table [expenses].[Links]    Script Date: 23.09.2015 20:45:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [expenses].[Links](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[URL] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Links] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) TEXTIMAGE_ON [PRIMARY]

GO


