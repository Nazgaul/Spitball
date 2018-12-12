using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Questions.Commands.FlagQuestion
{
    public class FlagQuestionCommandHandler : ICommandHandler<FlagQuestionCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Question> _qustionRepository;

        public FlagQuestionCommandHandler(IRepository<RegularUser> userRepository, IRepository<Question> qustionRepository)
        {
            _userRepository = userRepository;
            _qustionRepository = qustionRepository;
        }
        public async Task ExecuteAsync(FlagQuestionCommand message, CancellationToken token)
        {
            if (!ItemComponent.ValidateFlagReason(message.FlagReason))
            {
                throw new ArgumentException("reason is too long");
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (!Privileges.CanFlag(user.Score))
            {
                throw new UnauthorizedAccessException("not enough score");
            }
            var qustion = await _qustionRepository.LoadAsync(message.QuestionId, token);
            if (qustion.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot flag your own document");
            }

            qustion.Item.State = ItemState.Flagged;
            qustion.Item.FlagReason = message.FlagReason;
            await _qustionRepository.UpdateAsync(qustion, token);
        }
    }
}
