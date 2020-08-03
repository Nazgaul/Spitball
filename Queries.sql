
Need to upload dbi

-- v4

ALTER TABLE [sb].[course2]
    ADD [Description] /*new_column_name*/ nvarchar(max) /*new_column_datatype*/ NULL /*new_column_nullability*/
GO

ALTER TABLE [sb].[course2]
    ADD [State] /*new_column_name*/ nvarchar(255) /*new_column_datatype*/ NULL /*new_column_nullability*/
GO

ALTER TABLE [sb].[Course2]
    ADD [Create] /*new_column_name*/ DATETIME2(7) /*new_column_datatype*/ NULL /*new_column_nullability*/
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sb].[CourseEnrollment](
	[Id] [uniqueidentifier] NOT NULL,
	[Receipt] [nvarchar](255) NULL,
	[Create] [datetime2](7) NOT NULL,
	[CourseId] [bigint] NOT NULL,
	[UserId] [bigint] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [sb].[CourseEnrollment] ADD PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO
ALTER TABLE [sb].[CourseEnrollment]  WITH CHECK ADD  CONSTRAINT [FK_C056858D] FOREIGN KEY([UserId])
REFERENCES [sb].[User] ([Id])
GO
ALTER TABLE [sb].[CourseEnrollment] CHECK CONSTRAINT [FK_C056858D]
GO
ALTER TABLE [sb].[CourseEnrollment]  WITH CHECK ADD  CONSTRAINT [FK_E1C4F99F] FOREIGN KEY([CourseId])
REFERENCES [sb].[Course2] ([Id])
GO
ALTER TABLE [sb].[CourseEnrollment] CHECK CONSTRAINT [FK_E1C4F99F]
GO


Need to upload dbi

update sb.Course2
set [Description] = 'This course consists only of prepared content. You will be able to begin the course whenever you want, and proceed at your own pace. There are no deadlines for completing the work.'
where [Description] is null