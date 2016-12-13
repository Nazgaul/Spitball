namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Quiz
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Seo")]
        public const string QuizSeoQuery = @"
select u.country as Country
,b.boxname as BoxName, u.universityname as UniversityName, q.Name,
		(select top(1) qq.Text from zbox.QuizQuestion qq where QuizId = q.id) as FirstQuestion,
		q.Url
		,l.name as departmentName
        from 
		zbox.quiz q
		join zbox.box b on q.boxid= b.boxid
		left join zbox.University u on b.University = u.Id
		left join zbox.library l on b.libraryid = l.libraryid
		where q.id = @QuizId
        and q.publish = 1
        and q.isdeleted = 0;";

        public const string QuizQuery =
            @"select q.Id, q.Name, u.UserId as OwnerId, u.UserName as Owner, 
q.CreationTime as Date, q.NumberOfViews, q.Banner, q.Publish
from zbox.quiz q 
join zbox.Users u on q.UserId = u.UserId
 where q.id = @QuizId
 and q.isdeleted = 0;
";




        public const string Question = @"select q.Id, q.Text,q.RightAnswerId as correctAnswer from zbox.QuizQuestion q where QuizId = @QuizId;";

        public const string Answer = @"select a.id, a.text,a.QuestionId from zbox.QuizAnswer a where QuizId = @QuizId;";



        public const string UserQuiz = @"
select q.TimeTaken,q.Score ,q2.id
from zbox.SolvedQuiz q , zbox.quizlike q2 
where q.QuizId = @QuizId and q.UserId = @UserId
and q2.QuizId = @QuizId and q2.UserId = @UserId;";

        public const string UserAnswer = @"select q.AnswerId,q.QuestionId from zbox.SolvedQuestion q where QuizId = @QuizId and UserId = @UserId;";




        public const string TopUsers = @"select username as Name, UserImageLarge as Image 
  from zbox.users where userid in (
  select top(@topusers) userid from zbox.[SolvedQuiz] where quizid = @QuizId order by score desc)";

        public const string Discussion = @"select qd.Id,
u.UserName,
qd.CreationTime as Date,
u.UserImageLarge as UserPicture,
u.UserId,
u.Url as UserUrl,
qd.Text,
qd.QuestionId
from zbox.QuizDiscussion qd
left join zbox.Users u on u.UserId = qd.UserId
where qd.QuizId = @QuizId
order by qd.id desc";


        public const string NumberOfQuizSolved = @"select count(*) from zbox.SolvedQuiz
where quizid = @QuizId";


        public const string GetBoxQuiz = @"  select 
    id as Id, 
    q.userid as OwnerId,
    name as Name,
    Publish,
    LikeCount as Likes,
    NumberOfViews as NumOfViews,
    q.CreationTime as Date
    from zbox.quiz q
    where boxid = @BoxId
    and q.isdeleted = 0
	order by q.Id desc
	offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY";

    }
}
