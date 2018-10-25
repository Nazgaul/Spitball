---V7
-- need to create signalr service for production
-- need to update application insight connection string
-- need to update db connection string to support v2 functions

CREATE TABLE [sb].[Course](
	[Name] [nvarchar](255) NOT NULL,
	[Count] [int] NOT NULL
	
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

--populate courses tags
insert into sb.Course(Name,Count)
select distinct BoxName,0 from zbox.box
where isdeleted = 0
and discriminator in (2,3)
except
select Name,0 from sb.Course


insert into sb.Course(Name,Count)

select distinct CourseCode,0 from zbox.box
where isdeleted = 0
and CourseCode is not null
and discriminator in (2,3)
except
select Name,0 from sb.Course


ALTER TABLE sb.question
ADD Language NVARCHAR(5);

--run update in program.cs - to update question language UpdateLanguageAsync


/****** Object:  Table [sb].[University]    Script Date: 23/10/2018 16:24:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [sb].[University](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Extra] [nvarchar](255) NULL,
	[Country] [nvarchar](2) NOT NULL,
	[CreationTime] [datetime2](7) NULL,
	[UpdateTime] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Country] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO



ALTER TABLE sb.[user]
ADD [UniversityId2] [uniqueidentifier] NULL;
ALTER TABLE [sb].[User]  WITH CHECK ADD  CONSTRAINT [User_University2] FOREIGN KEY([UniversityId2])
REFERENCES [sb].[University] ([Id])



ALTER TABLE sb.[University]
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = ON)  
--Run TransferUniversities