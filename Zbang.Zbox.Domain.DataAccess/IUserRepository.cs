using System;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Enums;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByEmail(string iEmail);
        User GetUserByMembershipId(Guid membershipId);
        User GetUserByFacebookId(long facebookUserId);

        UserRelationshipType GetUserToBoxRelationShipType(long userId, long boxId);
        UserBoxRel GetUserBoxRelationship(long userId, long boxId);

        //UserRelationshipType GetUserToBoxRelationShipTypeWithInvite(long userId, long boxId);
        long GetItemsByUser(long userId);
        //User GetOwnerByBoxId(long boxId);

        bool IsNotUsedCode(string code, long userId);
    }
}
