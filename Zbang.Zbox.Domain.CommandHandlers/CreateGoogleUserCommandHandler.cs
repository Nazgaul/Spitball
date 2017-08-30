using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateGoogleUserCommandHandler : CreateUserCommandHandler
    {
        public CreateGoogleUserCommandHandler(IUserRepository userRepository,
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
            var googleCommand = command as CreateGoogleUserCommand;
            if (googleCommand == null)
            {
                throw new InvalidCastException("cant cast user to facebook user");
            }
            if (string.IsNullOrWhiteSpace(command.Email))
            {
                throw new NullReferenceException("email cannot be null or empty");
            }

            var user = GetUserByEmail(command.Email);
            if (user != null && user.Email == command.Email)
            {
                user.GoogleId = googleCommand.GoogleId;
                var retVal = new CreateGoogleUserCommandResult(user);
                if (user.University != null)
                {
                    retVal.UniversityData = user.University.UniversityData.Id;
                    retVal.UniversityId = user.University.Id;
                }
                return retVal;
            }
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
                user = UserRepository.GetUserByGoogleId(googleCommand.GoogleId);
                if (user != null && IsUserRegistered(user))
                {
                    throw new ArgumentException("user is already registered");
                }
                if (user == null)
                {
                    user = CreateUser(command.Email, googleCommand.Image,
                    command.FirstName,
                    command.LastName, command.Culture, command.Sex);
                    UserRepository.Save(user, true);
                    user.GenerateUrl();
                }
            }
            await UpdateUserAsync(user, command);

            user.GoogleId = googleCommand.GoogleId;

            var result = new CreateGoogleUserCommandResult(user);
            UpdateUniversityByBox(command.BoxId, result, user);
            UpdateUniversity(googleCommand, result, user);

            GiveReputationAndAssignToBox(command.InviteId, user);
            UserRepository.Save(user);
            return result;
        }
    }
}
