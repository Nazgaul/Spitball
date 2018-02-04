using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UnFollowBoxCommandHandler : ICommandHandlerAsync<UnFollowBoxCommand>
    {
        private readonly IRepository<Box> _boxRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQueueProvider _queueProvider;
        private readonly IUpdatesRepository _updatesRepository;

        public UnFollowBoxCommandHandler(IRepository<Box> boxRepository,
            IUserRepository userRepository,
            IQueueProvider queueProvider, IUpdatesRepository updatesRepository)
        {
            _boxRepository = boxRepository;
            _userRepository = userRepository;
            _queueProvider = queueProvider;
            _updatesRepository = updatesRepository;
        }

        public async Task HandleAsync(UnFollowBoxCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var box = _boxRepository.Load(message.BoxId);
            var user = _userRepository.Load(message.UserId);

            var academicBox = box.Actual as AcademicBox;
            if (academicBox != null)
            {
                await UnFollowBoxAsync(box, message.UserId);
                return;
            }

            var userType = _userRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId);
            if (userType == UserRelationshipType.Owner)
            {
                await DeleteBoxAsync(box, user);
                return;
            }
            if (userType == UserRelationshipType.Subscribe)
            {
                await UnFollowBoxAsync(box, message.UserId);
            }
        }

        private Task DeleteBoxAsync(Box box, User user)
        {
            box.UserTime.UpdateUserTime(user.Id);
            box.IsDeleted = true;
            _boxRepository.Save(box);
            return _queueProvider.InsertMessageToTransactionAsync(new DeleteBoxData(box.Id));
        }

        private Task UnFollowBoxAsync(Box box, long userId)
        {
            box.UnFollowBox(userId);
            _updatesRepository.DeleteUserUpdateByBoxId(userId, box.Id);
            _boxRepository.Save(box);
            return _queueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id));
        }
    }
}
