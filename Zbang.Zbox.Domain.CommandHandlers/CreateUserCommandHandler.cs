
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
        protected readonly IQueueProvider QueueRepository;
        protected readonly IRepository<University> UniversityRepository;
        protected readonly IInviteToCloudentsRepository InviteToCloudentsRepository;
        protected readonly IRepository<Reputation> ReputationRepository;

        protected CreateUserCommandHandler(IUserRepository userRepository,
            IQueueProvider queueRepository,
            IRepository<University> universityRepository,
            IInviteToCloudentsRepository inviteToCloudentsRepository,
            IRepository<Reputation> reputationRepository)
        {
            UserRepository = userRepository;
            QueueRepository = queueRepository;
            UniversityRepository = universityRepository;
            InviteToCloudentsRepository = inviteToCloudentsRepository;
            ReputationRepository = reputationRepository;
        }

        public virtual CreateUserCommandResult Execute(CreateUserCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            User user = UserRepository.GetUserByEmail(command.Email);
            var result = new CreateUserCommandResult(user);
            return result;
        }

        protected void TriggerWelcomeMail(User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            QueueRepository.InsertMessageToMailNew(new WelcomeMailData(user.Email, user.Name, user.Culture));
        }

        protected void AddReputation(User user)
        {
            var invite = InviteToCloudentsRepository.GetInviteToCloudents(user);
            if (invite == null)
            {
                return;
            }
            var reputation = invite.Sender.AddReputation(Infrastructure.Enums.ReputationAction.Invite);
            ReputationRepository.Save(reputation);
            UserRepository.Save(invite.Sender);
        }


        protected void AddReputaion()
        {

        }
        protected void UpdateUniversity(long universityId, CreateUserCommandResult result, User user)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (user == null) throw new ArgumentNullException("user");
            University university = UniversityRepository.Get(universityId);
            if (university == null)
            {
                return;
            }
            if (university.DataUnversity != null)
            {

                result.UniversityId = university.DataUnversity.Id;
                result.UniversityWrapperId = university.Id;
            }
            else
            {
                result.UniversityId = university.Id;
            }
            user.UpdateUserUniversity(university, string.Empty, null, null, null);
        }



    }
}
