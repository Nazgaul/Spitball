using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateFacebookUserCommandHandler : CreateUserCommandHandler
    {
        public CreateFacebookUserCommandHandler(IUserRepository userRepository,
            IQueueProvider queueRepository,
            IRepository<University> universityRepository,
            IRepository<InviteToSystem> inviteToCloudentsRepository,
            IRepository<Reputation> reputationRepository,
            IRepository<AcademicBox> academicBoxRepository)
            : base(userRepository, queueRepository, universityRepository, inviteToCloudentsRepository, reputationRepository, academicBoxRepository)
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
            GiveReputation(command.InviteId);


            var user = GetUserByEmail(command.Email);
            if (user != null && IsUserRegistered(user))
            {
                throw new ArgumentException("user is already registered");
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
                    user = CreateUser(command.Email, facebookCommand.UserImage, facebookCommand.LargeUserImage,
                    command.FirstName,
                    facebookCommand.MiddleName,
                    command.LastName, command.Sex, command.MarketEmail, command.Culture);
                    UserRepository.Save(user, true);
                    user.GenerateUrl();
                }
            }
            await UpdateUser(user, command);

            user.FacebookId = facebookCommand.FacebookUserId;

            var retVal = new CreateFacebookUserCommandResult(user);
            UpdateUniversityByBox(command.BoxId, retVal, user);
            if (facebookCommand.UniversityId.HasValue)
            {
                UpdateUniversity(facebookCommand.UniversityId.Value, retVal, user);
            }
            UserRepository.Save(user);
            return retVal;
        }
    }
}
