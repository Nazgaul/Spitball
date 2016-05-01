﻿using System;
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
            IInviteRepository inviteToCloudentsRepository,
            IRepository<Reputation> reputationRepository,
            IRepository<Box> boxRepository)
            : base(userRepository, queueRepository, universityRepository, inviteToCloudentsRepository, reputationRepository,
                  boxRepository)
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



            var user = GetUserByEmail(command.Email);

            if (user != null && IsUserRegistered(user))
            {
                throw new ArgumentException("user is already registered");
            }
            //var newUser = false;
            if (user == null)//email was invited to a box new user
            {
                user = CreateUser(command.Email,
                    null,
                    command.FirstName,
                    command.LastName, command.Culture,command.Sex);
                UserRepository.Save(user, true);
                user.GenerateUrl();
            }
            await UpdateUserAsync(user, command);

            user.MembershipId = membershipCommand.MembershipUserId;

            var retVal = new CreateMembershipUserCommandResult(user);
            UpdateUniversityByBox(membershipCommand.BoxId, retVal, user);
            if (membershipCommand.UniversityId.HasValue)
            {
                UpdateUniversity(membershipCommand.UniversityId.Value, retVal, user);

            }
            await GiveReputationAndAssignToBoxAsync(command.InviteId, user);
            UserRepository.Save(user);
            return retVal;
        }

    }
}
