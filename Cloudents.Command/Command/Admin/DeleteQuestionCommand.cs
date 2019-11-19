namespace Cloudents.Command.Command.Admin
{
    public class DeleteQuestionCommand : ICommand
    {
        public DeleteQuestionCommand(long questionId)
        {
            QuestionId = questionId;
        }

        public long QuestionId { get; }

    }
}