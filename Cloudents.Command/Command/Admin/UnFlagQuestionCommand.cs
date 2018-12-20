using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class UnFlagQuestionCommand : ICommand
    {
        public UnFlagQuestionCommand(long questionId)
        {
            QuestionId = questionId ;
        }
        
        public long QuestionId { get; }
    }
}
