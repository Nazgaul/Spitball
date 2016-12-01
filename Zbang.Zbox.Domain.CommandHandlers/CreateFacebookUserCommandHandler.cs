using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateFacebookUserCommandHandler : CreateUserCommandHandler
    {
        public CreateFacebookUserCommandHandler(IUserRepository userRepository,
            IQueueProvider queueRepository,
            IRepository<University> universityRepository,
            IInviteRepository inviteToCloudentsRepository,
            //IRepository<Reputation> reputationRepository,
            IRepository<Box> boxRepository)
            : base(userRepository, queueRepository, universityRepository, inviteToCloudentsRepository,
                   boxRepository)
        {

        }
        public override async Task<CreateUserCommandResult> ExecuteAsync(CreateUserCommand command)
        {
            var facebookCommand = command as CreateFacebookUserCommand;
            if (facebookCommand == null)
            {
                throw new InvalidCastException("cant cast user to facebook user");
            }
            if (string.IsNullOrWhiteSpace(command.Email))
            {
                throw new NullReferenceException("email cannot be null or empty");
            }


            var user = GetUserByEmail(command.Email);
            if (user != null && IsUserRegistered(user))
            {
                if (user.FacebookId.HasValue)
                {
                    throw new UserRegisterFacebookException();
                }
                if (!string.IsNullOrEmpty(user.GoogleId))
                {
                    throw new UserRegisterGoogleException();
                }
                throw new UserRegisterEmailException();
            }
            if (user == null)//email was invited to a box new user
            {
                user = UserRepository.GetUserByFacebookId(facebookCommand.FacebookUserId); // facebook invite
                if (user != null && IsUserRegistered(user))
                {
                    throw new ArgumentException("user is already registered");
                }
                if (user == null)
                {
                    user = CreateUser(command.Email, facebookCommand.LargeUserImage,
                    command.FirstName,
                    command.LastName, command.Culture, command.Sex);
                    UserRepository.Save(user, true);
                    user.GenerateUrl();
                }
            }
            await UpdateUserAsync(user, command);

            user.FacebookId = facebookCommand.FacebookUserId;
            if (command.Email.Contains("@facebook.com"))
            {
                user.EmailSendSettings = Infrastructure.Enums.EmailSend.NoSend;
            }
            var retVal = new CreateFacebookUserCommandResult(user);
            UpdateUniversityByBox(command.BoxId, retVal, user);
            UpdateUniversity(facebookCommand, retVal, user);

            GiveReputationAndAssignToBox(command.InviteId, user);
            UserRepository.Save(user);
            return retVal;
        }
    }
}
