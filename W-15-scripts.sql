﻿---V7
-- need to create signalr service for production
-- need to update application insight connection string
-- need to update db connection string to support v2 functions
-- need to change the app setting in production
-- new azure function
-- change blob storage to private

ALTER TABLE sb.course
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = ON)  

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

ALTER TABLE sb.[University]
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = ON)  
--Run TransferUniversities


insert into sb.Tag
select DISTINCT [Name], 0 
from Zbox.Tag
where len(Name) >= 4

ALTER TABLE sb.[Document]
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = ON)  

insert into sb.HiLoGenerator values('Document',0);

-- after user and university population
update sb.[user]
set country = (select country from  sb.University u2 where universityid2 = u2.Id)
where OldUser = 1

--need to remove cloudents from bing custom search



