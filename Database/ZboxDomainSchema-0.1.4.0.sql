/****** Object:  Table [dbo].[Storage]    Script Date: 05/01/2011 16:22:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Storage](
	[StorageId] [int] IDENTITY(1,1) NOT NULL,
	[CreationTimeUtc] [datetime] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[TotalSize] [bigint] NULL,
	[UsedSpace] [bigint] NULL,
 CONSTRAINT [PK_Storage] PRIMARY KEY CLUSTERED 
(
	[StorageId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Friend]    Script Date: 05/01/2011 16:22:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friend](
	[FriendId] [int] IDENTITY(1,1) NOT NULL,
	[FriendEmailAddress] [nvarchar](400) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Friends] PRIMARY KEY CLUSTERED 
(
	[FriendId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Box]    Script Date: 05/01/2011 16:22:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Box](
	[BoxId] [int] IDENTITY(1,1) NOT NULL,
	[BoxName] [nvarchar](50) NOT NULL,
	[CreationTimeUtc] [datetime] NULL,
	[StorageId] [int] NULL,
	[PrivacySettings] [int] NULL,
	[SharePassword] [nvarchar](255) NULL,
 CONSTRAINT [PK_Box] PRIMARY KEY CLUSTERED 
(
	[BoxId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[File]    Script Date: 05/01/2011 16:22:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[File](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](256) NOT NULL,
	[CreationTimeUtc] [datetime] NOT NULL,
	[BoxId] [int] NULL,
	[BlobAddressUri] [nvarchar](255) NOT NULL,
	[ContentType] [nvarchar](50) NOT NULL,
	[Length] [bigint] NOT NULL,
	[ThumbnailBlobAddressUri] [nvarchar](255) NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Link]    Script Date: 05/01/2011 16:22:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Link](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](255) NOT NULL,
	[CreationTimeUtc] [datetime] NULL,
	[BoxId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[SharedBoxSubscribers]    Script Date: 05/01/2011 16:22:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SharedBoxSubscribers](
	[UserId] [uniqueidentifier] NOT NULL,
	[BoxId] [int] NOT NULL,
 CONSTRAINT [PK_SharedBoxSubscribers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[BoxId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Invitations]    Script Date: 05/01/2011 16:22:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invitations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[class] [nvarchar](255) NOT NULL,
	[RecipientEmail] [nvarchar](255) NOT NULL,
	[SenderUserId] [uniqueidentifier] NULL,
	[CreationTimeUtc] [datetime] NULL,
	[FriendId] [int] NULL,
	[BoxId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Default [DF_Friend_Deleted]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[Friend] ADD  CONSTRAINT [DF_Friend_Deleted]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  ForeignKey [FK_Box_Storage]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[Box]  WITH CHECK ADD  CONSTRAINT [FK_Box_Storage] FOREIGN KEY([StorageId])
REFERENCES [dbo].[Storage] ([StorageId])
GO
ALTER TABLE [dbo].[Box] CHECK CONSTRAINT [FK_Box_Storage]
GO
/****** Object:  ForeignKey [FKF85B8BE0ADB7EEA1]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[Box]  WITH CHECK ADD  CONSTRAINT [FKF85B8BE0ADB7EEA1] FOREIGN KEY([StorageId])
REFERENCES [dbo].[Storage] ([StorageId])
GO
ALTER TABLE [dbo].[Box] CHECK CONSTRAINT [FKF85B8BE0ADB7EEA1]
GO
/****** Object:  ForeignKey [FK1C2B095D38AAF587]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK1C2B095D38AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK1C2B095D38AAF587]
GO
/****** Object:  ForeignKey [FK9E87361138AAF587]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK9E87361138AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK9E87361138AAF587]
GO
/****** Object:  ForeignKey [FK9E87361171704336]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK9E87361171704336] FOREIGN KEY([SenderUserId])
REFERENCES [dbo].[aspnet_Membership] ([UserId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK9E87361171704336]
GO
/****** Object:  ForeignKey [FK9E873611BA5174BD]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK9E873611BA5174BD] FOREIGN KEY([FriendId])
REFERENCES [dbo].[Friend] ([FriendId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK9E873611BA5174BD]
GO
/****** Object:  ForeignKey [FK65B095F38AAF587]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[Link]  WITH CHECK ADD  CONSTRAINT [FK65B095F38AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[Link] CHECK CONSTRAINT [FK65B095F38AAF587]
GO
/****** Object:  ForeignKey [FK5DD9021838AAF587]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers]  WITH CHECK ADD  CONSTRAINT [FK5DD9021838AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[SharedBoxSubscribers] CHECK CONSTRAINT [FK5DD9021838AAF587]
GO
/****** Object:  ForeignKey [FK5DD902184AFBD512]    Script Date: 05/01/2011 16:22:18 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers]  WITH CHECK ADD  CONSTRAINT [FK5DD902184AFBD512] FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Membership] ([UserId])
GO
ALTER TABLE [dbo].[SharedBoxSubscribers] CHECK CONSTRAINT [FK5DD902184AFBD512]
GO
