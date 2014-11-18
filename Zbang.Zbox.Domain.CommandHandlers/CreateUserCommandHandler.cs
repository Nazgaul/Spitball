
using System;
using System.Threading;
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
        private readonly IRepository<InviteToSystem> m_InviteToCloudentsRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;

        protected CreateUserCommandHandler(IUserRepository userRepository,
            IQueueProvider queueRepository,
            IRepository<University> universityRepository,
            IRepository<InviteToSystem> inviteToCloudentsRepository,
            IRepository<Reputation> reputationRepository)
        {
            UserRepository = userRepository;
            m_QueueRepository = queueRepository;
            m_UniversityRepository = universityRepository;
            m_InviteToCloudentsRepository = inviteToCloudentsRepository;
            m_ReputationRepository = reputationRepository;
        }

        public abstract CreateUserCommandResult Execute(CreateUserCommand command);

        private void TriggerWelcomeMail(User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            m_QueueRepository.InsertMessageToMailNew(new WelcomeMailData(user.Email, user.Name, user.Culture));
        }

        protected void GiveReputation(Guid? invId)
        {
            if (!invId.HasValue)
            {
                return;
            }
            var invite = m_InviteToCloudentsRepository.Get(invId.Value); // Load won't give null
            if (invite == null)
            {
                return;
            }
            if (invite.IsUsed)
            {
                return;
            }
            var reputation = invite.Sender.AddReputation(invite.GiveAction());
            m_ReputationRepository.Save(reputation);
            invite.UsedInvite();
            UserRepository.Save(invite.Sender);
            m_InviteToCloudentsRepository.Save(invite);
        }


        protected User GetUserByEmail(string email)
        {
            return UserRepository.GetUserByEmail(email);
        }

        protected User CreateUser(string email, string image, string largeImage,
            string firstName, string middleName, string lastName,
            bool sex, bool marketEmail, string culture)
        {
            return new User(email, image, largeImage,
                   firstName,
                   middleName,
                   lastName, sex, this.marketEmail(marketEmail), culture);
        }

        protected bool IsUserRegistered(User user)
        {
            return user.IsRegisterUser;
        }

        protected void UpdateUser(User user, CreateUserCommand command)
        {
            user.IsRegisterUser = true;

            user.FirstName = command.FirstName;
            user.MiddleName = command.MiddleName;
            user.LastName = command.LastName;
            user.CreateName();
            user.Sex = command.Sex;
            user.MarketEmail = marketEmail(command.MarketEmail);
            TriggerWelcomeMail(user);
            user.Quota.AllocateStorage();
        }

        private bool marketEmail(bool marketEmail)
        {
            return marketEmail && System.Globalization.CultureInfo.CurrentCulture.Name.ToLower() == "he-il";
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
            result.UniversityData = university.UniversityData.Id;
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
