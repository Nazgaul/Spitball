
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
        const int InviteToCloudentsReputation = 20;
        protected readonly IUserRepository m_UserRepository;
        protected readonly IQueueProvider m_QueueRepository;
        protected readonly IUniversityRepository m_UniversityRepository;
        protected readonly IInviteToCloudentsRepository m_InviteToCloudentsRepository;

        protected CreateUserCommandHandler(IUserRepository userRepository,
            IQueueProvider queueRepository,
            IUniversityRepository universityRepository,
            IInviteToCloudentsRepository inviteToCloudentsRepository)
        {
            m_UserRepository = userRepository;
            m_QueueRepository = queueRepository;
            m_UniversityRepository = universityRepository;
            m_InviteToCloudentsRepository = inviteToCloudentsRepository;
        }

        public virtual CreateUserCommandResult Execute(CreateUserCommand command)
        {
            User user = m_UserRepository.GetUserByEmail(command.Email);
            CreateUserCommandResult result = new CreateUserCommandResult(user);
            return result;
        }

        protected void TriggerWelcomeMail(User user)
        {
            m_QueueRepository.InsertMessageToMailNew(new WelcomeMailData(user.Email, user.Name, user.Culture));
        }

        protected void AddReputation(User user)
        {
           var invite =  m_InviteToCloudentsRepository.GetInviteToCloudents(user);
           if (invite == null)
           {
               return;
           }
           invite.Sender.AddReputation(InviteToCloudentsReputation);
           m_UserRepository.Save(invite.Sender);
        }


        protected void AddReputaion()
        {

        }
        protected void UpdateUniversity(long universityId, CreateUserCommandResult result, User user)
        {
            University university = m_UniversityRepository.Get(universityId);
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
            user.UpdateUserUniversity(university, string.Empty);
        }
       

        
    }
}
