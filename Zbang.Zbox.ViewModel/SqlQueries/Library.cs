namespace Zbang.Zbox.ViewModel.SqlQueries
{
   public static class Library
   {
       public const string GetAcademicBoxesByNode = @"
 select b.boxid as Id,
    b.BoxName as boxName,
    b.pictureurl as BoxPicture, 
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
     l.Url as Url
     from zbox.Library l
    where l.Id = @UniversityId
    and l.parentid is null
    order by name;";

       public const string GetLibraryNodeDetails = @"select l.name as Name
	 ,coalesce( p.Url ,'/library')  as ParentUrl
	 from zbox.Library l 
	 left  join zbox.Library p on l.ParentId = p.LibraryId
	 where l.LibraryId = @LibraryId";

       public const string GetLibraryNodeWithParent = @"
 select l.libraryid as Id, l.Name as Name, l.NoOfBoxes as NoBoxes,
     l.Url as Url
     from zbox.Library l
    where l.Id = @UniversityId
    and l.parentid = @ParentId
    order by name;
";


   }
}
