using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public abstract class CreateUserCommandHandler : ICommandHandlerAsync<CreateUserCommand, CreateUserCommandResult>
    {

        protected readonly IUserRepository UserRepository;
        private readonly IQueueProvider m_QueueRepository;
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IInviteRepository m_InviteToCloudentsRepository;
        //private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IRepository<Box> m_BoxRepository;

        protected CreateUserCommandHandler(IUserRepository userRepository,
            IQueueProvider queueRepository,
            IRepository<University> universityRepository,
            IInviteRepository inviteToCloudentsRepository,
            //IRepository<Reputation> reputationRepository, 
            IRepository<Box> boxRepository)
        {
            UserRepository = userRepository;
            m_QueueRepository = queueRepository;
            m_UniversityRepository = universityRepository;
            m_InviteToCloudentsRepository = inviteToCloudentsRepository;
            // m_ReputationRepository = reputationRepository;
            m_BoxRepository = boxRepository;
        }

        public abstract Task<CreateUserCommandResult> ExecuteAsync(CreateUserCommand command);

        //TODO: change name
        private Task TriggerWelcomeMailAsync(User user, CreateUserCommand command)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var t4 = m_QueueRepository.InsertMessageToTranactionAsync(new RegisterBadgeData(user.Id));
            var t1 =  m_QueueRepository.InsertMessageToTranactionAsync(new NewUserData(user.Id, user.Name, user.Culture, user.Email, command.Parameters?["ref"]));
            return Task.WhenAll(t1, t4);
        }

        protected void GiveReputationAndAssignToBox(Guid? invId, User user)
        {
            var inviteId = invId ?? Guid.Empty;
            var invites = m_InviteToCloudentsRepository.GetUserInvites(user.Email, inviteId); // Load won't give null

            //var list = new List<Task>();
            foreach (var invite in invites)
            {
                if (invite.IsUsed)
                {
                    continue;
                }
                //var reputation = invite.Sender.AddReputation(invite.GiveAction());
                //m_ReputationRepository.Save(reputation);
                invite.UsedInvite();

                UserRepository.Save(invite.Sender);
                m_InviteToCloudentsRepository.Save(invite);

                var inviteToBox = invite as InviteToBox;
                if (inviteToBox != null)
                {
                    user.ChangeUserRelationShipToBoxType(inviteToBox.Box, UserRelationshipType.Subscribe);
                }
            }
            //await Task.WhenAll(list);
        }


        protected User GetUserByEmail(string email)
        {
            return UserRepository.GetUserByEmail(email);
        }

        protected User CreateUser(string email, string largeImage,
            string firstName, string lastName,
              string culture, Sex sex)
        {
            return new User(email, largeImage,
                   firstName,
                   lastName, culture, sex);
        }

        protected bool IsUserRegistered(User user)
        {
            return user.IsRegisterUser;
        }

        protected Task UpdateUserAsync(User user, CreateUserCommand command)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (command == null) throw new ArgumentNullException(nameof(command));
            user.IsRegisterUser = true;
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.Sex = command.Sex;
            user.CreateName();
            user.Quota.AllocateStorage();
            return TriggerWelcomeMailAsync(user, command);
        }




        protected void UpdateUniversity(CreateUserCommand command, CreateUserCommandResult result, User user)
        {

            if (result == null) throw new ArgumentNullException(nameof(result));
            if (user == null) throw new ArgumentNullException(nameof(user));

            //long universityId;

            if (!command.UniversityId.HasValue) return;
            var university = m_UniversityRepository.Get(command.UniversityId.Value);
            if (university == null)
            {
                return;
            }
            UpdateUniversity(university, result, user);
        }

        private static void UpdateUniversity(University university, CreateUserCommandResult result, User user)
        {
            result.UniversityId = university.Id;
            result.UniversityData = university.UniversityData.Id;
            user.UpdateUniversity(university, null);
        }

        protected void UpdateUniversityByBox(long? boxId, CreateUserCommandResult result, User user)
        {
            if (!boxId.HasValue)
            {
                return;
            }

            var box = m_BoxRepository.Get(boxId.Value);
            if (box == null)
            {
                return;
            }
            user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
            var academicBox = box as AcademicBox;
            if (academicBox != null)
            {
                UpdateUniversity(academicBox.University, result, user);
            }
        }



    }
}
