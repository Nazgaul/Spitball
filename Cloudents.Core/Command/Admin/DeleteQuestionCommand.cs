using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
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