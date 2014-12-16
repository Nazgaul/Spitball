using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateMembershipUserCommandHandler : CreateUserCommandHandler
    {

        public CreateMembershipUserCommandHandler(IUserRepository userRepository,
            IRepository<University> universityRepository,
            IQueueProvider queueRepository,
            IRepository<InviteToSystem> inviteToCloudentsRepository,
            IRepository<Reputation> reputationRepository,
            IRepository<AcademicBox> academicBoxRepository)
            : base(userRepository, queueRepository, universityRepository, inviteToCloudentsRepository, reputationRepository, academicBoxRepository)
        {
        }
        public override async Task<CreateUserCommandResult> ExecuteAsync(CreateUserCommand command)
        {
            var membershipCommand = command as CreateMembershipUserCommand;
            if (membershipCommand == null)
            {
                throw new InvalidCastException("cant cast user to membership user");
            }
            if (string.IsNullOrEmpty(command.Email))
            {
                throw new NullReferenceException("command.Email");
            }
            GiveReputation(command.InviteId);


            var user = GetUserByEmail(command.Email);

            if (user != null && IsUserRegistered(user))
            {
                throw new ArgumentException("user is already registered");
            }
            //var newUser = false;
            if (user == null)//email was invited to a box new user
            {
                user = CreateUser(command.Email,
                    null, null,
                    command.FirstName,
                    null,
                    command.LastName, command.Sex, command.MarketEmail, command.Culture);
                UserRepository.Save(user, true);
                user.GenerateUrl();
            }
            await UpdateUser(user, command);

            user.MembershipId = membershipCommand.MembershipUserId;

            var retVal = new CreateMembershipUserCommandResult(user);
            UpdateUniversityByBox(membershipCommand.BoxId, retVal, user);
            if (membershipCommand.UniversityId.HasValue)
            {
                UpdateUniversity(membershipCommand.UniversityId.Value, retVal, user);

            }
            UserRepository.Save(user);
            return retVal;
        }



        //private University GetUniversity(string universityName)
        //{
        //    if (string.IsNullOrEmpty(universityName))
        //    {
        //        return null;
        //    }
        //    var university = m_UniversityRepository.GetUniversity(universityName);
        //    //if (university == null)
        //    //{
        //    //    var images = m_ProfileProvider.GetDefaultProfileImage();
        //    //    university = new University(universityName, images.Image.AbsoluteUri, images.LargeImage.AbsoluteUri);

        //    //    m_UniversityRepository.Save(university, true);
        //    //}
        //    return university;
        //}
    }
}
