namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Quiz
    {

        //todo:change that
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Seo")]
        public const string QuizSeoQuery = @"
select u.country as Country
,b.boxname as BoxName, u.universityname as UniversityName, q.Name,
		(select top(1) qq.Text from zbox.QuizQuestion qq where QuizId = q.id) as FirstQuestion,
		q.Url
        from 
		zbox.quiz q
		join zbox.box b on q.boxid= b.boxid
		join zbox.users u on b.ownerid = u.userid
		where id = @QuizId
        and q.publish = 1;";

        public const string QuizQuery =
            @"select q.Id, q.Name, u.UserId as OwnerId, u.UserName as Owner, q.CreationTime as Date, q.NumberOfViews,
b.BoxName, b.Url as BoxUrl, q.Publish , [Stdevp] , Average
from zbox.quiz q 
join zbox.Users u on q.UserId = u.UserId
join zbox.Box b on q.BoxId = b.BoxId
 where q.id = @QuizId
";

        public const string Question = @"select q.Id, q.Text,q.RightAnswerId as correctAnswer from zbox.QuizQuestion q where QuizId = @QuizId;";

        public const string Answer = @"select a.id, a.text,a.QuestionId from zbox.QuizAnswer a where QuizId = @QuizId;";



        public const string UserQuiz = @"
select q.TimeTaken,q.Score from zbox.SolvedQuiz q where QuizId = @QuizId and UserId = @UserId;";

        public const string UserAnswer = @"select q.AnswerId,q.QuestionId from zbox.SolvedQuestion q where QuizId = @QuizId and UserId = @UserId;";

        public const string TopUsers = @"  select username as Name, userimage as Image 
  from zbox.users where userid in (
  select top(3) userid from zbox.[SolvedQuiz] where quizid = @QuizId order by score desc)";

        public const string Discussion = @"select qd.Id,
u.UserName,
qd.CreationTime as Date,
u.UserImage as UserPicture,
u.UserId,
qd.Text,
qd.QuestionId
from zbox.QuizDiscussion qd
left join zbox.Users u on u.UserId = qd.UserId
where qd.QuizId = @QuizId";


        public const string NumberOfQuizSolved = @"select count(*) from zbox.SolvedQuiz
where quizid = @QuizId";
    }
}
