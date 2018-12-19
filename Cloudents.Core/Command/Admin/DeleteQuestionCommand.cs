using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class DeleteQuestionCommand : ICommand
    {
        public DeleteQuestionCommand(long questionId)
        {
            QuestionId = questionId;
        }

        public long QuestionId { get;}

    }
}