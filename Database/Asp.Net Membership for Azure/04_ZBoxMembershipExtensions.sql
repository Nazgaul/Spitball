--Extension to Membership to support Mail Verification
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TABLE dbo.aspnet_Membership
ADD IsEmailVerified BIT NOT NULL
CONSTRAINT IsEmailVerified_Constraint DEFAULT 0
GO
