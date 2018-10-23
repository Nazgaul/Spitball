---V7
-- need to create signalr service for production

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

--run update in program.cs - to update question language UpdateLanguageAsync