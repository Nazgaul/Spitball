USE [ZBox]
GO
/****** Object:  Table [dbo].[Storage]    Script Date: 06/26/2011 01:29:50 ******/
CREATE TABLE [dbo].[Storage](
	[StorageId] [int] IDENTITY(1,1) NOT NULL,
	[CreationTimeUtc] [datetime] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[TotalSize] [bigint] NULL,
	[UsedSpace] [bigint] NULL,
 CONSTRAINT [PK_Storage] PRIMARY KEY CLUSTERED 
(
	[StorageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
GO
/****** Object:  Table [dbo].[Friend]    Script Date: 06/26/2011 01:29:12 ******/
CREATE TABLE [dbo].[Friend](
	[FriendId] [int] IDENTITY(1,1) NOT NULL,
	[FriendEmailAddress] [nvarchar](400) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Friends] PRIMARY KEY CLUSTERED 
(
	[FriendId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
GO
/****** Object:  Table [dbo].[Box]    Script Date: 06/26/2011 01:28:52 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
GO
/****** Object:  Table [dbo].[File]    Script Date: 06/26/2011 01:29:06 ******/
CREATE TABLE [dbo].[File](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](256) NOT NULL,
	[CreationTimeUtc] [datetime] NOT NULL,
	[BoxId] [int] NULL,
	[BlobAddressUri] [nvarchar](255) NOT NULL,
	[ContentType] [nvarchar](50) NOT NULL,
	[Length] [bigint] NOT NULL,
	[ThumbnailBlobAddressUri] [nvarchar](255) NULL,
	[CreationTimeEpochMillis] [bigint] NULL,
	[IsDeleted] [bit] NULL,
	[UploaderId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
GO
/****** Object:  Table [dbo].[NotificationRules]    Script Date: 06/26/2011 01:29:36 ******/
CREATE TABLE [dbo].[NotificationRules](
	[NotificationRuleId] [int] IDENTITY(1,1) NOT NULL,
	[notificationSetting] [int] NULL,
	[UserId] [uniqueidentifier] NULL,
	[BoxId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[NotificationRuleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
GO
/****** Object:  Table [dbo].[Link]    Script Date: 06/26/2011 01:29:30 ******/
CREATE TABLE [dbo].[Link](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](2000) NOT NULL,
	[CreationTimeUtc] [datetime] NULL,
	[BoxId] [int] NOT NULL,
	[CreationTimeEpochMillis] [bigint] NULL,
	[IsDeleted] [bit] NULL,
	[UploaderId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
GO
/****** Object:  Table [dbo].[SharedBoxSubscribers]    Script Date: 06/26/2011 01:29:42 ******/
CREATE TABLE [dbo].[SharedBoxSubscribers](
	[UserId] [uniqueidentifier] NOT NULL,
	[BoxId] [int] NOT NULL,
	[SubscriberId] [int] NULL,
	[permission] [int] NULL,
 CONSTRAINT [PK_SharedBoxSubscribers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
GO
/****** Object:  Table [dbo].[Invitations]    Script Date: 06/26/2011 01:29:19 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
GO
/****** Object:  Default [ColumnDefault_45710ed8-084c-4ad1-b361-7997f0cc19ca]    Script Date: 06/26/2011 01:29:08 ******/
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [ColumnDefault_45710ed8-084c-4ad1-b361-7997f0cc19ca]  DEFAULT ((0)) FOR [CreationTimeEpochMillis]
GO
/****** Object:  Default [DF__File__IsDeleted__23D353CA]    Script Date: 06/26/2011 01:29:09 ******/
ALTER TABLE [dbo].[File] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
/****** Object:  Default [DF_Friend_Deleted]    Script Date: 06/26/2011 01:29:13 ******/
ALTER TABLE [dbo].[Friend] ADD  CONSTRAINT [DF_Friend_Deleted]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [ColumnDefault_64e3a194-9698-4c67-8dab-cff83efe850e]    Script Date: 06/26/2011 01:29:32 ******/
ALTER TABLE [dbo].[Link] ADD  CONSTRAINT [ColumnDefault_64e3a194-9698-4c67-8dab-cff83efe850e]  DEFAULT ((0)) FOR [CreationTimeEpochMillis]
GO
/****** Object:  Default [ColumnDefault_2a06f27b-7996-455a-8268-770287d98eab]    Script Date: 06/26/2011 01:29:33 ******/
ALTER TABLE [dbo].[Link] ADD  CONSTRAINT [ColumnDefault_2a06f27b-7996-455a-8268-770287d98eab]  DEFAULT ((0)) FOR [IsDeleted]
GO
/****** Object:  ForeignKey [FK_Box_Storage]    Script Date: 06/26/2011 01:28:53 ******/
ALTER TABLE [dbo].[Box]  WITH CHECK ADD  CONSTRAINT [FK_Box_Storage] FOREIGN KEY([StorageId])
REFERENCES [dbo].[Storage] ([StorageId])
GO
ALTER TABLE [dbo].[Box] CHECK CONSTRAINT [FK_Box_Storage]
GO
/****** Object:  ForeignKey [FK598FD9719C9EFC2B]    Script Date: 06/26/2011 01:28:54 ******/
ALTER TABLE [dbo].[Box]  WITH CHECK ADD  CONSTRAINT [FK598FD9719C9EFC2B] FOREIGN KEY([StorageId])
REFERENCES [dbo].[Storage] ([StorageId])
GO
ALTER TABLE [dbo].[Box] CHECK CONSTRAINT [FK598FD9719C9EFC2B]
GO
/****** Object:  ForeignKey [FKF85B8BE0ADB7EEA1]    Script Date: 06/26/2011 01:28:55 ******/
ALTER TABLE [dbo].[Box]  WITH CHECK ADD  CONSTRAINT [FKF85B8BE0ADB7EEA1] FOREIGN KEY([StorageId])
REFERENCES [dbo].[Storage] ([StorageId])
GO
ALTER TABLE [dbo].[Box] CHECK CONSTRAINT [FKF85B8BE0ADB7EEA1]
GO
/****** Object:  ForeignKey [FK1C2B095D38AAF587]    Script Date: 06/26/2011 01:29:07 ******/
ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK1C2B095D38AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK1C2B095D38AAF587]
GO
/****** Object:  ForeignKey [FK6F586F2C645BB10B]    Script Date: 06/26/2011 01:29:07 ******/
ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK6F586F2C645BB10B] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK6F586F2C645BB10B]
GO
/****** Object:  ForeignKey [FK7B4EAFBA645BB10B]    Script Date: 06/26/2011 01:29:20 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK7B4EAFBA645BB10B] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK7B4EAFBA645BB10B]
GO
/****** Object:  ForeignKey [FK7B4EAFBAC9E56681]    Script Date: 06/26/2011 01:29:21 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK7B4EAFBAC9E56681] FOREIGN KEY([FriendId])
REFERENCES [dbo].[Friend] ([FriendId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK7B4EAFBAC9E56681]
GO
/****** Object:  ForeignKey [FK7B4EAFBAE103802A]    Script Date: 06/26/2011 01:29:21 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK7B4EAFBAE103802A] FOREIGN KEY([SenderUserId])
REFERENCES [dbo].[aspnet_Membership] ([UserId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK7B4EAFBAE103802A]
GO
/****** Object:  ForeignKey [FK9E87361138AAF587]    Script Date: 06/26/2011 01:29:22 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK9E87361138AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK9E87361138AAF587]
GO
/****** Object:  ForeignKey [FK9E87361171704336]    Script Date: 06/26/2011 01:29:23 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK9E87361171704336] FOREIGN KEY([SenderUserId])
REFERENCES [dbo].[aspnet_Membership] ([UserId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK9E87361171704336]
GO
/****** Object:  ForeignKey [FK9E873611BA5174BD]    Script Date: 06/26/2011 01:29:24 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK9E873611BA5174BD] FOREIGN KEY([FriendId])
REFERENCES [dbo].[Friend] ([FriendId])
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK9E873611BA5174BD]
GO
/****** Object:  ForeignKey [FK65B095F38AAF587]    Script Date: 06/26/2011 01:29:31 ******/
ALTER TABLE [dbo].[Link]  WITH CHECK ADD  CONSTRAINT [FK65B095F38AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[Link] CHECK CONSTRAINT [FK65B095F38AAF587]
GO
/****** Object:  ForeignKey [FKD9C1B91E645BB10B]    Script Date: 06/26/2011 01:29:31 ******/
ALTER TABLE [dbo].[Link]  WITH CHECK ADD  CONSTRAINT [FKD9C1B91E645BB10B] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[Link] CHECK CONSTRAINT [FKD9C1B91E645BB10B]
GO
/****** Object:  ForeignKey [FK4DF61A4838AAF587]    Script Date: 06/26/2011 01:29:37 ******/
ALTER TABLE [dbo].[NotificationRules]  WITH CHECK ADD  CONSTRAINT [FK4DF61A4838AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[NotificationRules] CHECK CONSTRAINT [FK4DF61A4838AAF587]
GO
/****** Object:  ForeignKey [FK5DD9021838AAF587]    Script Date: 06/26/2011 01:29:43 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers]  WITH CHECK ADD  CONSTRAINT [FK5DD9021838AAF587] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[SharedBoxSubscribers] CHECK CONSTRAINT [FK5DD9021838AAF587]
GO
/****** Object:  ForeignKey [FK5DD902184AFBD512]    Script Date: 06/26/2011 01:29:43 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers]  WITH CHECK ADD  CONSTRAINT [FK5DD902184AFBD512] FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Membership] ([UserId])
GO
ALTER TABLE [dbo].[SharedBoxSubscribers] CHECK CONSTRAINT [FK5DD902184AFBD512]
GO
/****** Object:  ForeignKey [FKFDB063893EB124D5]    Script Date: 06/26/2011 01:29:44 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers]  WITH CHECK ADD  CONSTRAINT [FKFDB063893EB124D5] FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Membership] ([UserId])
GO
ALTER TABLE [dbo].[SharedBoxSubscribers] CHECK CONSTRAINT [FKFDB063893EB124D5]
GO
/****** Object:  ForeignKey [FKFDB06389645BB10B]    Script Date: 06/26/2011 01:29:45 ******/
ALTER TABLE [dbo].[SharedBoxSubscribers]  WITH CHECK ADD  CONSTRAINT [FKFDB06389645BB10B] FOREIGN KEY([BoxId])
REFERENCES [dbo].[Box] ([BoxId])
GO
ALTER TABLE [dbo].[SharedBoxSubscribers] CHECK CONSTRAINT [FKFDB06389645BB10B]
GO
