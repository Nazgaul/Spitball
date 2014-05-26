namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Updates
    {
        public const string GetUserUpdates = @"select BoxId,QuestionId,AnswerId,ItemId,QuizId from zbox.NewUpdates
where UserId = @userid";
    }
}
