USE [CandaWebUtility]
GO

/****** Object:  Table [dbo].[Logging]    Script Date: 2025-02-14 10:57:59 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Logging](
	[ROWID] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
	[DateTimeStamp] datetime NOT NULL Default getdate(),
	[User] [nvarchar](50) NOT NULL,
	[Action] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](2000) NOT NULL DEFAULT '',
	[FromValue] [nvarchar](2000) NOT NULL DEFAULT '',
	[ToValue] [nvarchar](2000) NOT NULL DEFAULT '',
) ON [PRIMARY]
GO





