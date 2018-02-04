using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RateItemCommandHandler : ICommandHandlerAsync<RateItemCommand>
    {
        private readonly IItemRateRepository _itemRateRepository;
        private readonly IRepository<Item> _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQueueProvider _queueProvider;

        public RateItemCommandHandler(
            IItemRateRepository itemRateRepository,
            IRepository<Item> itemRepository,
            IUserRepository userRepository, IQueueProvider queueProvider)
        {
            _itemRateRepository = itemRateRepository;
            _itemRepository = itemRepository;
            _userRepository = userRepository;
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(RateItemCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var userRate = _itemRateRepository.GetRateOfUser(message.UserId, message.ItemId);

            var item = _itemRepository.Load(message.ItemId);
            item.ShouldMakeDirty = () => true;

            if (userRate != null)
            {
                _itemRateRepository.Delete(userRate);
                item.LikeCount--;
                _itemRepository.Save(item);
                return ReturnValueAsync(item.UploaderId, message.UserId);
            }
            var user = _userRepository.Load(message.UserId);
            userRate = new ItemRate(user, item, message.Id);
            _itemRateRepository.Save(userRate);
            item.LikeCount++;
            _itemRepository.Save(item);
            return ReturnValueAsync(item.UploaderId, message.UserId);
        }

        private Task ReturnValueAsync(long userId, long userWhoMadeAction)
        {
            var t2 = _queueProvider.InsertMessageToTransactionAsync(new LikesBadgeData(userWhoMadeAction));
            var t1 = _queueProvider.InsertMessageToTransactionAsync(new ReputationData(userId));
            return Task.WhenAll(t1, t2);
        }


    }
}
