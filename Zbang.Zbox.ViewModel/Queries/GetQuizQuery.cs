namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetQuizQuery : QueryBase
    {
        public GetQuizQuery(long quizId, long userId, long boxId)
            : base(userId)
        {
            QuizId = quizId;
            BoxId = boxId;
        }
        public long QuizId { get; private set; }
        public long BoxId { get; private set; }

    }

    public class GetQuizSeoQuery
    {
        public GetQuizSeoQuery(long quizId)
        {
            QuizId = quizId;
        }

        public long QuizId { get; private set; }
    }

    public class GetQuizDraftQuery
    {
        public GetQuizDraftQuery(long quizId)
        {
            QuizId = quizId;
        }
        public long QuizId { get; private set; }
    }

    public class GetDisscussionQuery
    {
        public GetDisscussionQuery(long quizId)
        {
            QuizId = quizId;
        }
        public long QuizId { get; private set; }
    }
}
