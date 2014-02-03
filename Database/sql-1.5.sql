/****** Object:  ForeignKey [FK_Box_Storage]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Box] DROP CONSTRAINT [FK_Box_Storage]
GO
/****** Object:  ForeignKey [FKF85B8BE0ADB7EEA1]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Box] DROP CONSTRAINT [FKF85B8BE0ADB7EEA1]
GO
/****** Object:  ForeignKey [FK_BoxShare_Box]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[BoxShare] DROP CONSTRAINT [FK_BoxShare_Box]
GO
/****** Object:  ForeignKey [FK_BoxShare_Friend]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[BoxShare] DROP CONSTRAINT [FK_BoxShare_Friend]
GO
/****** Object:  ForeignKey [FK1C2B095D38AAF587]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK1C2B095D38AAF587]
GO
/****** Object:  ForeignKey [FK9E87361138AAF587]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Invitations] DROP CONSTRAINT [FK9E87361138AAF587]
GO
/****** Object:  ForeignKey [FK9E87361171704336]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Invitations] DROP CONSTRAINT [FK9E87361171704336]
GO
/****** Object:  ForeignKey [FK9E873611BA5174BD]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Invitations] DROP CONSTRAINT [FK9E873611BA5174BD]
GO
/****** Object:  ForeignKey [FK65B095F38AAF587]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Link] DROP CONSTRAINT [FK65B095F38AAF587]
GO
/****** Object:  ForeignKey [FK5DD9021838AAF587]    Script Date: 05/10/2011 06:53:09 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers] DROP CONSTRAINT [FK5DD9021838AAF587]
GO
/****** Object:  ForeignKey [FK5DD902184AFBD512]    Script Date: 05/10/2011 06:53:09 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers] DROP CONSTRAINT [FK5DD902184AFBD512]
GO
/****** Object:  Table [dbo].[Invitations]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Invitations] DROP CONSTRAINT [FK9E87361138AAF587]
GO
ALTER TABLE [dbo].[Invitations] DROP CONSTRAINT [FK9E87361171704336]
GO
ALTER TABLE [dbo].[Invitations] DROP CONSTRAINT [FK9E873611BA5174BD]
GO
DROP TABLE [dbo].[Invitations]
GO
/****** Object:  Table [dbo].[SharedBoxSubscribers]    Script Date: 05/10/2011 06:53:09 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers] DROP CONSTRAINT [FK5DD9021838AAF587]
GO
ALTER TABLE [dbo].[SharedBoxSubscribers] DROP CONSTRAINT [FK5DD902184AFBD512]
GO
DROP TABLE [dbo].[SharedBoxSubscribers]
GO
/****** Object:  Table [dbo].[Link]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Link] DROP CONSTRAINT [FK65B095F38AAF587]
GO
DROP TABLE [dbo].[Link]
GO
/****** Object:  Table [dbo].[BoxShare]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[BoxShare] DROP CONSTRAINT [FK_BoxShare_Box]
GO
ALTER TABLE [dbo].[BoxShare] DROP CONSTRAINT [FK_BoxShare_Friend]
GO
DROP TABLE [dbo].[BoxShare]
GO
/****** Object:  Table [dbo].[File]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK1C2B095D38AAF587]
GO
ALTER TABLE [dbo].[File] DROP CONSTRAINT [DF_File_CreationTimeEpochMillis]
GO
DROP TABLE [dbo].[File]
GO
/****** Object:  Table [dbo].[Box]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Box] DROP CONSTRAINT [FK_Box_Storage]
GO
ALTER TABLE [dbo].[Box] DROP CONSTRAINT [FKF85B8BE0ADB7EEA1]
GO
DROP TABLE [dbo].[Box]
GO
/****** Object:  Table [dbo].[BoxDto]    Script Date: 05/10/2011 06:53:08 ******/
DROP TABLE [dbo].[BoxDto]
GO
/****** Object:  Table [dbo].[FileDto]    Script Date: 05/10/2011 06:53:08 ******/
DROP TABLE [dbo].[FileDto]
GO
/****** Object:  Table [dbo].[Friend]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Friend] DROP CONSTRAINT [DF_Friend_Deleted]
GO
DROP TABLE [dbo].[Friend]
GO
/****** Object:  Table [dbo].[FriendDto]    Script Date: 05/10/2011 06:53:08 ******/
DROP TABLE [dbo].[FriendDto]
GO
/****** Object:  Table [dbo].[LinkDto]    Script Date: 05/10/2011 06:53:08 ******/
DROP TABLE [dbo].[LinkDto]
GO
/****** Object:  Table [dbo].[QuotaDto]    Script Date: 05/10/2011 06:53:08 ******/
DROP TABLE [dbo].[QuotaDto]
GO
/****** Object:  Table [dbo].[Storage]    Script Date: 05/10/2011 06:53:09 ******/
DROP TABLE [dbo].[Storage]
GO
/****** Object:  Table [dbo].[StorageDto]    Script Date: 05/10/2011 06:53:09 ******/
DROP TABLE [dbo].[StorageDto]
GO
/****** Object:  Table [dbo].[SubscribedBoxDto]    Script Date: 05/10/2011 06:53:09 ******/
DROP TABLE [dbo].[SubscribedBoxDto]
GO
/****** Object:  Table [dbo].[SubscribedBoxDto]    Script Date: 05/10/2011 06:53:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscribedBoxDto](
	[BoxId] [int] IDENTITY(1,1) NOT NULL,
	[BoxName] [nvarchar](255) NOT NULL,
	[BoxOwnerEmail] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BoxId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[StorageDto]    Script Date: 05/10/2011 06:53:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StorageDto](
	[StorageId] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StorageId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Storage]    Script Date: 05/10/2011 06:53:09 ******/
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
/****** Object:  Table [dbo].[QuotaDto]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuotaDto](
	[StorageId] [int] IDENTITY(1,1) NOT NULL,
	[TotalSize] [float] NOT NULL,
	[UsedSpace] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StorageId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[LinkDto]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LinkDto](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](255) NOT NULL,
	[CreationTimeUtc] [nvarchar](255) NULL,
	[CreationTimeUtcMillis] [bigint] NULL,
	[CreationTimeEpochMillis] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[FriendDto]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FriendDto](
	[FriendId] [int] IDENTITY(1,1) NOT NULL,
	[FriendEmailAddress] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FriendId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Friend]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friend](
	[FriendId] [int] IDENTITY(1,1) NOT NULL,
	[FriendEmailAddress] [nvarchar](400) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Friend_Deleted]  DEFAULT ((1)),
 CONSTRAINT [PK_Friends] PRIMARY KEY CLUSTERED 
(
	[FriendId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[FileDto]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileDto](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[BlobAddressUri] [nvarchar](255) NOT NULL,
	[ThumbnailBlobAddressUri] [nvarchar](255) NOT NULL,
	[CreationTimeUtc] [nvarchar](255) NULL,
	[ContentType] [nvarchar](255) NULL,
	[Length] [bigint] NULL,
	[CreationTimeEpochMillis] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[BoxDto]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BoxDto](
	[BoxId] [int] IDENTITY(1,1) NOT NULL,
	[BoxName] [nvarchar](255) NOT NULL,
	[PrivacySettings] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BoxId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Box]    Script Date: 05/10/2011 06:53:08 ******/
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
/****** Object:  Table [dbo].[File]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[File](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](256) NOT NULL,
	[CreationTimeUtc] [datetime] NOT NULL,
	[CreationTimeEpochMillis] [bigint] NULL CONSTRAINT [DF_File_CreationTimeEpochMillis]  DEFAULT ((0)),
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
/****** Object:  Table [dbo].[BoxShare]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BoxShare](
	[BoxShareId] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[FriendId] [int] NOT NULL,
 CONSTRAINT [PK_BoxShare] PRIMARY KEY CLUSTERED 
(
	[BoxShareId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Link]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Link](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](255) NOT NULL,
	[CreationTimeUtc] [datetime] NULL,
	[CreationTimeEpochMillis] [bigint] NULL,
	[BoxId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[SharedBoxSubscribers]    Script Date: 05/10/2011 06:53:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SharedBoxSubscribers](
	[BoxId] [int] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BoxId] ASC,
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Invitations]    Script Date: 05/10/2011 06:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invitations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[class] [nvarchar](255) NOT NULL,
	[RecipientEmail] [nvarchar](255) NOT NULL,
	[SenderUserId] [uniqueidentifier] NOT NULL,
	[CreationTimeUtc] [datetime] NULL,
	[FriendId] [int] NULL,
	[BoxId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  ForeignKey [FK_Box_Storage]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Box]  WITH CHECK ADD  CONSTRAINT [FK_Box_Storage] FOREIGN KEY([StorageId])
REFERENCES [dbo].[Storage] ([StorageId])
GO
ALTER TABLE [dbo].[Box] CHECK CONSTRAINT [FK_Box_Storage]
GO
/****** Object:  ForeignKey [FKF85B8BE0ADB7EEA1]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Box]  WITH CHECK ADD  CONSTRAINT [FKF85B8BE0ADB7EEA1] FOREIGN KEY([StorageId])
REFERENCES [dbo].[Storage] ([StorageId])
GO
ALTER TABLE [dbo].[Box] CHECK CONSTRAINT [FKF85B8BE0ADB7EEA1]
GO
/****** Object:  ForeignKey [FK_BoxShare_Box]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[BoxShare]  WITH CHECK ADD  CONSTRAINT [FK_BoxShare_Box] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[BoxShare] CHECK CONSTRAINT [FK_BoxShare_Box]
GO
/****** Object:  ForeignKey [FK_BoxShare_Friend]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[BoxShare]  WITH CHECK ADD  CONSTRAINT [FK_BoxShare_Friend] FOREIGN KEY([FriendId])
REFERENCES [dbo].[Friend] ([FriendId])
GO
ALTER TABLE [dbo].[BoxShare] CHECK CONSTRAINT [FK_BoxShare_Friend]
GO
/****** Object:  ForeignKey [FK1C2B095D38AAF587]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK1C2B095D38AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK1C2B095D38AAF587]
GO
/****** Object:  ForeignKey [FK9E87361138AAF587]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK9E87361138AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK9E87361138AAF587]
GO
/****** Object:  ForeignKey [FK9E87361171704336]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK9E87361171704336] FOREIGN KEY([SenderUserId])
REFERENCES [dbo].[aspnet_Membership] ([UserId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK9E87361171704336]
GO
/****** Object:  ForeignKey [FK9E873611BA5174BD]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK9E873611BA5174BD] FOREIGN KEY([FriendId])
REFERENCES [dbo].[Friend] ([FriendId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK9E873611BA5174BD]
GO
/****** Object:  ForeignKey [FK65B095F38AAF587]    Script Date: 05/10/2011 06:53:08 ******/
ALTER TABLE [dbo].[Link]  WITH CHECK ADD  CONSTRAINT [FK65B095F38AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[Link] CHECK CONSTRAINT [FK65B095F38AAF587]
GO
/****** Object:  ForeignKey [FK5DD9021838AAF587]    Script Date: 05/10/2011 06:53:09 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers]  WITH CHECK ADD  CONSTRAINT [FK5DD9021838AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[SharedBoxSubscribers] CHECK CONSTRAINT [FK5DD9021838AAF587]
GO
/****** Object:  ForeignKey [FK5DD902184AFBD512]    Script Date: 05/10/2011 06:53:09 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers]  WITH CHECK ADD  CONSTRAINT [FK5DD902184AFBD512] FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Membership] ([UserId])
GO
ALTER TABLE [dbo].[SharedBoxSubscribers] CHECK CONSTRAINT [FK5DD902184AFBD512]
GO
