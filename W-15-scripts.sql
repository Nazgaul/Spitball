---V7
-- need to create signalr service for production

CREATE TABLE [sb].[Course](
	[Name] [nvarchar](255) NOT NULL,
	[Count] [int] NOT NULL,
	[Extra] [nvarchar](4000) NULL,
PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE sb.course
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = ON)  

CREATE TABLE [sb].[UsersCourses](
	[User_id] [bigint] NOT NULL,
	[Course_id] [nvarchar](255) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [sb].[UsersCourses]  WITH CHECK ADD  CONSTRAINT [Courses_User] FOREIGN KEY([Course_id])
REFERENCES [sb].[Course] ([Name])
GO

ALTER TABLE [sb].[UsersCourses] CHECK CONSTRAINT [Courses_User]
GO

ALTER TABLE [sb].[UsersCourses]  WITH CHECK ADD  CONSTRAINT [User_Courses] FOREIGN KEY([User_id])
REFERENCES [sb].[User] ([Id])
GO

ALTER TABLE [sb].[UsersCourses] CHECK CONSTRAINT [User_Courses]
GO