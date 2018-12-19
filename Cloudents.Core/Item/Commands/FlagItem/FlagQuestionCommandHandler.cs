using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Item.Commands.FlagItem
{
    public class FlagQuestionCommandHandler : BaseFlagItemCommandHandler<Question,long>, ICommandHandler<FlagQuestionCommand>
    {

        public FlagQuestionCommandHandler(IRepository<RegularUser> userRepository,
            IRepository<Question> questionRepository):base(questionRepository, userRepository)
        {
        }
        public async Task ExecuteAsync(FlagQuestionCommand message, CancellationToken token)
        {
            await base.ExecuteAsync(message, token);
        }

        protected override void Validate(Question question, User user)
        {
            if (question.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot flag your own document");
            }
        }
    }
}
