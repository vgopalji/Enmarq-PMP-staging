USE [CareStream]
GO

ALTER TABLE [dbo].[BulkUser] DROP CONSTRAINT [FK_BulkUserFile_BulkUser]
GO

/****** Object:  Table [dbo].[BulkUser]    Script Date: 5/26/2020 10:57:32 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BulkUser]') AND type in (N'U'))
DROP TABLE [dbo].[BulkUser]
GO

/****** Object:  Table [dbo].[BulkUser]    Script Date: 5/26/2020 10:57:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BulkUser](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FileId] [bigint] NOT NULL,
	[Action] [nvarchar](15) NOT NULL,
	[Status] [nvarchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ObjectID] [nvarchar](100) NULL,
	[UserPrincipalName] [nvarchar](100) NULL,
	[DisplayName] [nvarchar](50) NULL,
	[BlockSignIn] [bit] NULL,
	[InitialPassword] [nvarchar](25) NULL,
	[FirstName] [nvarchar](25) NULL,
	[LastName] [nvarchar](25) NULL,
	[JobTitle] [nvarchar](25) NULL,
	[Department] [nvarchar](50) NULL,
	[Usagelocation] [nvarchar](25) NULL,
	[StreetAddress] [nvarchar](255) NULL,
	[State] [nvarchar](25) NULL,
	[Country] [nvarchar](25) NULL,
	[Office] [nvarchar](50) NULL,
	[City] [nvarchar](50) NULL,
	[ZIP] [nvarchar](10) NULL,
	[OfficePhone] [nvarchar](25) NULL,
	[MobilePhone] [nvarchar](25) NULL,
	[InviteeEmail] [nvarchar](25) NULL,
	[InviteRedirectURL] [nvarchar](50) NULL,
	[SendEmail] [bit] NULL,
	[CustomizedMessageBody] [nvarchar](255) NULL,
	[Error] [nvarchar](max) NULL,
 CONSTRAINT [PK_BulkUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BulkUser]  WITH CHECK ADD  CONSTRAINT [FK_BulkUserFile_BulkUser] FOREIGN KEY([FileId])
REFERENCES [dbo].[BulkUserFile] ([Id])
GO

ALTER TABLE [dbo].[BulkUser] CHECK CONSTRAINT [FK_BulkUserFile_BulkUser]
GO


