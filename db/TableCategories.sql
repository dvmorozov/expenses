
/****** Object:  Table [expenses].[Categories]    Script Date: 04.11.2015 10:45:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [expenses].[Categories](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [char](100) NOT NULL,
	[DataOwner] [uniqueidentifier] NOT NULL,
	[Limit] [float] NULL,
	[EncryptedName] [nvarchar](max) NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
