using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.Item.Commands.FlagItem
{
    public abstract class BaseFlagAnItem<T, TId> where T : ItemObject
    {
        private readonly IRepository<T> _repository;
        private readonly IRepository<RegularUser> _userRepository;

        protected BaseFlagAnItem(IRepository<T> repository, IRepository<RegularUser> userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }
        protected async Task ExecuteAsync(BaseFlagItemCommand<TId> message, CancellationToken token)
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
            var answer = await _repository.LoadAsync(message.Id, token);

            Validate(answer, user);
            //if (answer.User.Id == user.Id)
            //{
            //    throw new UnauthorizedAccessException("you cannot flag your own document");
            //}

            answer.Item.State = ItemState.Flagged;
            answer.Item.FlagReason = message.FlagReason;
            answer.Item.FlaggedUserId = user.Id;
            await _repository.UpdateAsync(answer, token);
        }

        protected abstract void Validate(T obj, User user);


    }
}