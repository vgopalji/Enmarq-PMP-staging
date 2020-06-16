USE [CareStream]
GO

/****** Object:  Table [dbo].[BulkUserFile]    Script Date: 5/26/2020 10:55:44 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BulkUserFile]') AND type in (N'U'))
DROP TABLE [dbo].[BulkUserFile]
GO

/****** Object:  Table [dbo].[BulkUserFile]    Script Date: 5/26/2020 10:55:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BulkUserFile](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](50) NOT NULL,
	[FileSize] [nvarchar](50) NOT NULL,
	[UploadBy] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[Action] [nvarchar](20) NOT NULL,
	[Status] [nvarchar](20) NULL,
	[Error] [nvarchar](max) NULL,
 CONSTRAINT [PK_BulkUserFile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


