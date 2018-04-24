
-- =============================================
-- Author:		D.V.Morozov
-- Create date: 12/10/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/7e2676fe-39fd-4290-bd26-17a2b4b7af7e
-- =============================================

ALTER TABLE [expenses].Expenses ADD Importance SMALLINT;
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 10/10/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/49348fc0-3dc6-45cb-8425-6fe72042eac2
-- =============================================

ALTER TABLE [expenses].Expenses ADD Rating SMALLINT;
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 11/07/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
-- =============================================

ALTER TABLE [expenses].Expenses ADD Currency NCHAR(5);
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 18/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
-- =============================================

ALTER TABLE [expenses].Expenses ADD EncryptedName NVARCHAR(MAX);
GO

ALTER TABLE [expenses].Categories ADD EncryptedName NVARCHAR(MAX);
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 12/06/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
-- =============================================

ALTER TABLE [expenses].Expenses ADD Monthly BIT NULL;
GO

ALTER TABLE [expenses].Expenses ADD FirstMonth DATE NULL;
GO

ALTER TABLE [expenses].Expenses ADD LastMonth DATE NULL;
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 04/11/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
-- =============================================

CREATE NONCLUSTERED INDEX [IX_Expenses_DataOwner] ON [expenses].[Expenses]
(
	[DataOwner] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Expenses_Name] ON [expenses].[Expenses]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [Idx_Expenses] ON [expenses].[Expenses]
(
	[DataOwner] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- =============================================
--	Change type of EncryptedName column.
ALTER TABLE [expenses].Expenses ADD EncryptedNameFixed NVARCHAR(MAX) NULL;
GO

UPDATE Expenses
SET EncryptedNameFixed = EncryptedName;
GO

--SELECT EncryptedNameFixed
--FROM Expenses

ALTER TABLE [expenses].Expenses DROP COLUMN EncryptedName;
GO

ALTER TABLE [expenses].Expenses ADD EncryptedName NVARCHAR(MAX) NULL;
GO

UPDATE Expenses
SET EncryptedName = EncryptedNameFixed;
GO

ALTER TABLE [expenses].Expenses DROP COLUMN EncryptedNameFixed;
GO

-- =============================================
--	Change type of Name column.
ALTER TABLE [expenses].Expenses ADD NameFixed NVARCHAR(MAX) NOT NULL DEFAULT '';
GO

UPDATE [expenses].Expenses
SET NameFixed = Name;
GO

ALTER TABLE [expenses].Expenses DROP COLUMN Name;
GO

ALTER TABLE [expenses].Expenses ADD Name NVARCHAR(MAX) NOT NULL DEFAULT '';
GO

UPDATE [expenses].Expenses
SET Name = NameFixed;
GO

ALTER TABLE [expenses].Expenses DROP COLUMN NameFixed;
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 05/11/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
-- =============================================

ALTER TABLE [expenses].Expenses ADD NameChecksum INT NULL;
GO

UPDATE [expenses].Expenses
SET NameChecksum = CHECKSUM(Name, EncryptedName)


CREATE NONCLUSTERED INDEX [IX_Expenses_NameChecksum] ON [expenses].[Expenses]
(
	[NameChecksum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- =============================================
-- Author:		D.V.Morozov
-- Create date: 08/11/2015
-- Description:	https://www.evernote.com/shard/s132/nl/14501366/333c0ad2-6962-4de1-93c1-591aa92bbcb3
-- =============================================

ALTER TABLE [expenses].Expenses ADD Project NVARCHAR(MAX);
GO
