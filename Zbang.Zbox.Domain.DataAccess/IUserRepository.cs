﻿using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByEmail(string iEmail);
        User GetUserByMembershipId(Guid membershipId);
        User GetUserByFacebookId(long facebookUserId);
        User GetUserByGoogleId(string googleUserId);

        UserRelationshipType GetUserToBoxRelationShipType(long userId, long boxId);

        UserLibraryRelationType GetUserToDepartmentRelationShipType(long userId, Guid departmentId);
        UserBoxRel GetUserBoxRelationship(long userId, long boxId);

        //UserRelationshipType GetUserToBoxRelationShipTypeWithInvite(long userId, long boxId);
        long GetItemsByUser(long userId);
        //User GetOwnerByBoxId(long boxId);

        bool IsNotUsedCode(string code, long userId);
        void UpdateUserReputation(int reputation, long userid);
        void RegisterUserNotification(long userid, MobileOperatingSystem os);
        void UnsubscibeUserFromMail(IEnumerable<string> emails);
    }
}
