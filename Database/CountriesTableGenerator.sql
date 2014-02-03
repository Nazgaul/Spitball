USE [ZBox]
GO

/****** Object:  Table [Zbox].[Countries]    Script Date: 12/10/2012 12:07:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Zbox].[Countries](
	[CountryId] [bigint] IDENTITY(1,1) NOT NULL,
	[CountryCode] [nvarchar](10) NOT NULL,
	[CountryName_Eng] [nvarchar](100) NOT NULL,
	[CountryName_Heb] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[CountryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

