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
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUpdatesRepository m_UpdatesRepository;

        public UnFollowBoxCommandHandler(IRepository<Box> boxRepository,
            IUserRepository userRepository,
            IQueueProvider queueProvider, IUpdatesRepository updatesRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
            m_UpdatesRepository = updatesRepository;
        }

        public async Task HandleAsync(UnFollowBoxCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var box = m_BoxRepository.Load(message.BoxId);
            var user = m_UserRepository.Load(message.UserId);

            var academicBox = box.Actual as AcademicBox;
            if (academicBox != null)
            {
                await UnFollowBoxAsync(box, message.UserId);
                return;
            }

            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId);
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
            m_BoxRepository.Save(box);
            return m_QueueProvider.InsertMessageToTransactionAsync(new DeleteBoxData(box.Id));
        }

        private Task UnFollowBoxAsync(Box box, long userId)
        {
            box.UnFollowBox(userId);
            m_UpdatesRepository.DeleteUserUpdateByBoxId(userId, box.Id);
            m_BoxRepository.Save(box);
            return m_QueueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id));
        }
    }
}
