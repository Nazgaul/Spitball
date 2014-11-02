
using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public abstract class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserCommandResult>
    {

        protected readonly IUserRepository UserRepository;
        private readonly IQueueProvider m_QueueRepository;
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IInviteToCloudentsRepository m_InviteToCloudentsRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;

        protected CreateUserCommandHandler(IUserRepository userRepository,
            IQueueProvider queueRepository,
            IRepository<University> universityRepository,
            IInviteToCloudentsRepository inviteToCloudentsRepository,
            IRepository<Reputation> reputationRepository)
        {
            UserRepository = userRepository;
            m_QueueRepository = queueRepository;
            m_UniversityRepository = universityRepository;
            m_InviteToCloudentsRepository = inviteToCloudentsRepository;
            m_ReputationRepository = reputationRepository;
        }

        public abstract CreateUserCommandResult Execute(CreateUserCommand command);
        //{
        //    if (command == null) throw new ArgumentNullException("command");
        //    User user = UserRepository.GetUserByEmail(command.Email);
        //    var result = new CreateUserCommandResult(user);
        //    return result;
        //}

        protected void TriggerWelcomeMail(User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            m_QueueRepository.InsertMessageToMailNew(new WelcomeMailData(user.Email, user.Name, user.Culture));
        }

        protected void AddReputation(User user)
        {
            var invite = m_InviteToCloudentsRepository.GetInviteToCloudents(user);
            if (invite == null)
            {
                return;
            }
            var reputation = invite.Sender.AddReputation(Infrastructure.Enums.ReputationAction.Invite);
            m_ReputationRepository.Save(reputation);
            UserRepository.Save(invite.Sender);
        }

        protected User GetUserByInviteId(Guid? invId)
        {
            if (!invId.HasValue)
            {
                return null;
            }
            var invite = m_InviteToCloudentsRepository.Load(invId.Value);
            if (invite.Recipient.IsRegisterUser)
            {
                return null;
            }
            return invite.Recipient;
        }



        protected void UpdateUniversity(long universityId, CreateUserCommandResult result, User user)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (user == null) throw new ArgumentNullException("user");
            var university = m_UniversityRepository.Get(universityId);
            if (university == null)
            {
                return;
            }
            result.UniversityId = university.Id;
            user.UpdateUniversity(university, null, null, null, null);
        }

        //protected User CreateUser()
        //{
        //    var defaultImages = m_ProfileProvider.GetDefaultProfileImage();
        //    user = new User(command.Email, defaultImages.Image.AbsoluteUri, defaultImages.LargeImage.AbsoluteUri,
        //        command.FirstName,
        //        command.MiddleName,
        //        command.LastName, command.Sex, command.MarketEmail, command.Culture);
        //    UserRepository.Save(user, true);
        //    user.GenerateUrl();
        //}

    }
}
