using System;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateMembershipUserCommandHandler : CreateUserCommandHandler
    {
        private readonly IProfilePictureProvider m_ProfileProvider;

        public CreateMembershipUserCommandHandler(IUserRepository userRepository,
            IProfilePictureProvider profileProvider,
            IRepository<University> universityRepository,
            IQueueProvider queueRepository,
            IInviteToCloudentsRepository inviteToCloudentsRepository,
            IRepository<Reputation> reputationRepository)
            : base(userRepository, queueRepository, universityRepository, inviteToCloudentsRepository, reputationRepository)
        {
            m_ProfileProvider = profileProvider;
        }
        public override CreateUserCommandResult Execute(CreateUserCommand command)
        {
            var membershipCommand = command as CreateMembershipUserCommand;
            if (membershipCommand == null)
            {
                throw new InvalidCastException("cant cast user to membership user");
            }
            Throw.OnNull(command.Email, "Email");


            User user = UserRepository.GetUserByEmail(command.Email);
            var newUser = false;
            if (user == null)//email was invited to a box new user
            {
                newUser = true;
                var defaultImages = m_ProfileProvider.GetDefaultProfileImage();
                user = new User(command.Email, defaultImages.Image.AbsoluteUri, defaultImages.LargeImage.AbsoluteUri,
                    command.FirstName,
                    command.MiddleName,
                    command.LastName, command.Sex, command.MarketEmail);
                UserRepository.Save(user, true);
                user.GenerateUrl();
            }
            if (!user.IsRegisterUser)
            {
                TriggerWelcomeMail(user);
                user.IsRegisterUser = true;

                user.FirstName = command.FirstName;
                user.MiddleName = command.MiddleName;
                user.LastName = command.LastName;
                user.CreateName();
                user.Sex = command.Sex;
                user.MarketEmail = command.MarketEmail;

                user.Quota.AllocateStorage();
                if (!newUser)
                {
                    AddReputation(user);
                }
            }
            user.MembershipId = membershipCommand.MembershipUserId;

            var retVal = new CreateMembershipUserCommandResult(user);
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
