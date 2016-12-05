namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public class Dashboard
    {
        public const string UserBoxes = @"select b.boxid as id,
                                b.BoxName as Name,
                                ub.UserType, 
                                b.quizcount + b.itemcount + COALESCE(b.FlashcardCount,0) as ItemCount,
                                b.MembersCount as MembersCount,
                                b.commentcount as CommentCount,
                                b.CourseCode,
                                b.ProfessorName as Professor,
                                b.Discriminator as boxType,
								b.Url as Url,
                                b.LibraryId as departmentId
    from Zbox.box b join zbox.UserBoxRel ub on b.BoxId = ub.BoxId  
    where ub.UserId = @UserId
    and ub.usertype in (2,3)
    and b.IsDeleted = 0
    ORDER BY b.UpdateTime desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

    }
}
