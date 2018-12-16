using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Item.Commands.FlagItem
{
    public class FlagQuestionCommand : BaseFlagItemCommand<long>
    {
        public FlagQuestionCommand(long userId, long questionId, string flagReason)
            : base(userId, questionId, flagReason)
        {
        }
    }
}
