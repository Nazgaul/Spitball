namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetQuizSeoQuery
    {
        public GetQuizSeoQuery(long quizId)
        {
            QuizId = quizId;
        }

        public long QuizId { get; private set; }
    }
}