ALTER TABLE [ZBox].[dbo].[File]
ADD
	IsDeleted bit DEFAULT 0;
GO

ALTER TABLE [ZBox].[dbo].[File]
ADD
	UploaderId uniqueidentifier;
GO

UPDATE [ZBox].[dbo].[File] SET
 [File].UploaderId = [aspnet_Users].UserId,
 [File].IsDeleted = 0
FROM [ZBox].[dbo].[File]
 INNER JOIN [ZBox].[dbo].[Box] ON [Box].BoxId = [File].BoxId
 INNER JOIN [ZBox].[dbo].[Storage] ON [Storage].StorageId = [Box].StorageId
 INNER JOIN [Zbox].[dbo].[aspnet_Users] ON [aspnet_Users].UserId = [Storage].UserId
GO


ALTER TABLE [ZBox].[dbo].[Link]
ADD
	IsDeleted bit DEFAULT 0;
GO

ALTER TABLE [ZBox].[dbo].[Link]
ADD
	UploaderId uniqueidentifier;
GO

UPDATE [ZBox].[dbo].[Link] SET
 [Link].UploaderId = [aspnet_Users].UserId,
 [Link].IsDeleted = 0
FROM [ZBox].[dbo].[Link]
 INNER JOIN [ZBox].[dbo].[Box] ON [Box].BoxId = [Link].BoxId
 INNER JOIN [ZBox].[dbo].[Storage] ON [Storage].StorageId = [Box].StorageId
 INNER JOIN [Zbox].[dbo].[aspnet_Users] ON [aspnet_Users].UserId = [Storage].UserId

GO


