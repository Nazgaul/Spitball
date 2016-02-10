namespace Zbang.Zbox.ViewModel.SqlQueries
{
   public static class Library
   {
       public const string GetAcademicBoxesByNode = @"
 select b.boxid as Id,
    b.BoxName as boxName,
    (select ub2.UserType from zbox.UserBoxRel ub2 where ub2.userid = @UserId and ub2.boxid = b.BoxId) as UserType ,
    b.quizcount + b.itemcount as ItemCount,
    b.MembersCount as MembersCount,
    b.commentcount as CommentCount,
    b.CourseCode as CourseCode,
    b.ProfessorName as ProfessorName,    
    b.Discriminator as BoxType,
    b.Url as Url
    from zbox.box b 
    where b.IsDeleted = 0  and b.discriminator = 2
    and  b.LibraryId = @ParentId
    ORDER BY  
          BoxName asc;";

       public const string GetLibraryNode = @" select l.libraryid as Id, l.Name as Name, l.NoOfBoxes as NoBoxes,
     l.AmountOfChildren as NoDepartment,
     l.Url as Url,
     l.settings as state,
	 case l.settings
	    when 1 then (select usertype from zbox.userlibraryrel where libraryid = l.LibraryId and userid = @Userid) 
		end as UserType
     from zbox.Library l
    where l.Id = @UniversityId
    and l.parentid is null
    order by name;";


       //TODO: check if we can do it better
       public const string GetLibraryNodeDetails = @"select l.name as Name
	 ,coalesce( p.Url ,'/library/')  as ParentUrl,
     l.settings as state
	 from zbox.Library l 
	 left  join zbox.Library p on l.ParentId = p.LibraryId
	 where l.LibraryId = @LibraryId";

       public const string GetLibraryNodeWithParent = @"
 select l.libraryid as Id, l.Name as Name, l.NoOfBoxes as NoBoxes,
    l.AmountOfChildren as NoDepartment,
     l.Url as Url,
     l.settings as state,
     case l.settings
	    when 1 then (
		select usertype from zbox.userlibraryrel where userid = 1 and libraryid in (
SELECT top 1
    libraryid
FROM zbox.Library
where (select level from zbox.library where Libraryid = @ParentId).IsDescendantOf(level) = 1
)
		) 
		end as UserType
     from zbox.Library l
    where l.Id = @UniversityId
    and l.parentid = @ParentId
    order by name;
";


   }
}
