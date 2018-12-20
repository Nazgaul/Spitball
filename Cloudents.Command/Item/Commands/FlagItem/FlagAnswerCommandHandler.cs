using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Item.Commands.FlagItem
{
    public class FlagAnswerCommandHandler : BaseFlagItemCommandHandler<Answer,Guid>, ICommandHandler<FlagAnswerCommand>
    {
        //private readonly IRepository<RegularUser> _userRepository;
        //private readonly IRepository<Answer> _answerRepository;

        public FlagAnswerCommandHandler(IRepository<RegularUser> userRepository,
            IRepository<Answer> answerRepository):base(answerRepository, userRepository)
        {
           // _userRepository = userRepository;
            //_answerRepository = answerRepository;
        }
        //public async Task ExecuteAsync(FlagAnswerCommand message, CancellationToken token)
        //{
        //    await ExecuteAsync(message, token);
        //    //if (!ItemComponent.ValidateFlagReason(message.FlagReason))
        //    //{
        //    //    throw new ArgumentException("reason is too long");
        //    //}
        //    //var user = await _userRepository.LoadAsync(message.UserId, token);
        //    //if (!Privileges.CanFlag(user.Score))
        //    //{
        //    //    throw new UnauthorizedAccessException("not enough score");
        //    //}
        //    //var answer = await _answerRepository.LoadAsync(message.AnswerId, token);

            

        //    //answer.Item.State = ItemState.Flagged;
        //    //answer.Item.FlagReason = message.FlagReason;
        //    //answer.Item.FlaggedUserId = user.Id;
        //    //await _answerRepository.UpdateAsync(answer, token);
        //}

        protected override void Validate(Answer answer, User user)
        {
            if (answer.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot flag your own document");
            }
        }

        public async Task ExecuteAsync(FlagAnswerCommand message, CancellationToken token)
        {
           await base.ExecuteAsync(message, token);
        }
    }
}
