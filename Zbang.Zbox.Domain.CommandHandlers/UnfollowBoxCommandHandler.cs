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
                //if (message.ShouldDelete && user.IsAdmin() && user.University.Id == academicBox.University.Id)
                //{
                //    await DeleteBox(box, user);
                //}
                //else
                //{
                    UnFollowBox(box, message.UserId);
                //}
                return;
            }

            //if (message.ShouldDelete && user.IsAdmin())
            //{
            //    var academicBox = box as AcademicBox;
            //    if (academicBox != null)
            //    {
            //        if (user.University.Id == academicBox.University.Id)
            //        {
            //            await DeleteBox(box, user);
            //        }
            //    }
            //}


            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId);
            if (userType == UserRelationshipType.Owner)
            {
                await DeleteBox(box, user);
                return;
            }
            if (userType == UserRelationshipType.Subscribe)
            {
                UnFollowBox(box, message.UserId);
            }


        }

        private Task DeleteBox(Box box, User user)
        {
            box.UserTime.UpdateUserTime(user.Email);
            box.IsDeleted = true;
            m_BoxRepository.Save(box);
            return m_QueueProvider.InsertMessageToTranactionAsync(new DeleteBoxData(box.Id));
        }
        private void UnFollowBox(Box box, long userId)
        {
            box.UnFollowBox(userId);
            m_UpdatesRepository.DeleteUserUpdateByBoxId(userId, box.Id);
            m_BoxRepository.Save(box);
        }
    }
}
