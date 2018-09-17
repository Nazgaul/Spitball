ALTER TABLE sb.[User]
ADD PasswordHash nvarchar(4000);
ALTER TABLE sb.[User]
ADD LockoutEnd datetimeoffset(7);
ALTER TABLE sb.[User]
ADD AccessFailedCount int;
ALTER TABLE sb.[User]
ADD LockoutEnabled bit;

CREATE TABLE [sb].[UserLogin](
	[LoginProvider] [nvarchar](255) NOT NULL,
	[ProviderKey] [nvarchar](255) NOT NULL,
	[ProviderDisplayName] [nvarchar](255) NULL,
	[UserId] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [sb].[UserLogin]  WITH CHECK ADD  CONSTRAINT [UserLogin_User] FOREIGN KEY([UserId])
REFERENCES [sb].[User] ([Id])
GO

ALTER TABLE [sb].[UserLogin] CHECK CONSTRAINT [UserLogin_User]
GO