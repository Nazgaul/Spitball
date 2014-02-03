﻿using System;
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
            IUniversityRepository universityRepository,
            IInviteToCloudentsRepository inviteToCloudentsRepository)
            : base(userRepository, queueRepository, universityRepository, inviteToCloudentsRepository)
        {

        }
        public override CreateUserCommandResult Execute(CreateUserCommand command)
        {
            var facebookCommand = command as CreateFacebookUserCommand;
            if (facebookCommand == null)
            {
                throw new InvalidCastException("cant cast user to facebook user");
            }
            if (string.IsNullOrWhiteSpace(command.Email))
            {
                throw new ArgumentNullException("email", "email cannot be null or empty");
            }

            User user = m_UserRepository.GetUserByEmail(command.Email);
            var newUser = false;
            if (user == null)//email was invited to a box new user
            {
                user = m_UserRepository.GetUserByFacebookId(facebookCommand.FacebookUserId); // facebook invite
                if (user == null)
                {
                    newUser = true;
                    user = new User(command.Email, command.UserName, facebookCommand.UserImage, facebookCommand.LargeUserImage);
                }
            }
            if (!user.IsRegisterUser)
            {
                user.Email = facebookCommand.Email;
                user.Name = command.UserName;
                TriggerWelcomeMail(user);
                user.IsRegisterUser = true;
                user.Quota.AllocateStorage();
                if (!newUser)
                {
                    AddReputation(user);
                }
            }
            user.FacebookId = facebookCommand.FacebookUserId;
            
            var retVal = new CreateFacebookUserCommandResult(user);
            if (facebookCommand.UniversityId.HasValue)
            {
                UpdateUniversity(command.UniversityId.Value, retVal, user);
            }
            m_UserRepository.Save(user);
            return retVal;
        }
    }
}
