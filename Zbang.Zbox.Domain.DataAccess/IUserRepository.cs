﻿using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByEmail(string iEmail);
        User GetUserByFacebookId(long facebookUserId);
        User GetUserByGoogleId(string googleUserId);

        UserRelationshipType GetUserToBoxRelationShipType(long userId, long boxId);

        UserLibraryRelationType GetUserToDepartmentRelationShipType(long userId, Guid departmentId);
        UserBoxRel GetUserBoxRelationship(long userId, long boxId);

        long GetItemsByUser(long userId);

        bool IsNotUsedCode(string code, long userId);
        //void UpdateUserReputation(int reputation, long userid);
        void UpdateScore(int score, long userid);
        void UnsubscribeUserFromMail(IEnumerable<string> emails, EmailSend type);
        int LikesCount(long userId);
        int ItemCount(long userId);
        int QuizCount(long userId);
        void UpdateUserFeedDetails(long userId);
    }
}
