namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public class Dashboard
    {
        /// <summary>
        /// Dashboard - get user boxes
        /// </summary>
        public const string UserBoxesOld = @"select b.boxid as id,
                                b.BoxName,
                                ub.UserType, 
                                b.quizcount + b.itemcount as ItemCount,
                                b.MembersCount as MembersCount,
                                b.commentcount as CommentCount,
                                b.CourseCode,
                                b.ProfessorName,
                                b.Discriminator as boxType,
								b.Url as Url,
    (select count(*) from zbox.NewUpdates n where boxid = b.BoxId and n.UserId = @UserId) as Updates
    from Zbox.box b join zbox.UserBoxRel ub on b.BoxId = ub.BoxId  
    where ub.UserId = @UserId
    and ub.usertype in (2,3)
    and b.IsDeleted = 0
    ORDER BY b.UpdateTime desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";



        public const string UserBoxes = @"select b.boxid as id,
                                b.BoxName,
                                ub.UserType, 
                                b.quizcount + b.itemcount as ItemCount,
                                b.MembersCount as MembersCount,
                                b.commentcount as CommentCount,
                                b.CourseCode,
                                b.ProfessorName,
                                b.Discriminator as boxType,
								b.Url as Url
    from Zbox.box b join zbox.UserBoxRel ub on b.BoxId = ub.BoxId  
    where ub.UserId = @UserId
    and ub.usertype in (2,3)
    and b.IsDeleted = 0
    ORDER BY b.UpdateTime desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

    }
}
