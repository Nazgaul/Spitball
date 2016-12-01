using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class UserRepository : NHibernateRepository<User>, IUserRepository
    {
        public User GetUserByEmail(string iEmail)
        {
            var criteria = UnitOfWork.CurrentSession.QueryOver<User>();
            criteria.Where(w => w.Email == iEmail.ToLower());

            return criteria.SingleOrDefault();
        }

        //public User GetUserByMembershipId(Guid membershipId)
        //{
        //    var criteria = UnitOfWork.CurrentSession.QueryOver<User>();
        //    criteria.Where(w => w.MembershipId == membershipId);

        //    return criteria.SingleOrDefault();
        //}
        public User GetUserByFacebookId(long facebookUserId)
        {
            var criteria = UnitOfWork.CurrentSession.QueryOver<User>();
            criteria.Where(w => w.FacebookId == facebookUserId);

            return criteria.SingleOrDefault();
        }

        public User GetUserByGoogleId(string googleUserId)
        {
            var criteria = UnitOfWork.CurrentSession.QueryOver<User>();
            criteria.Where(w => w.GoogleId == googleUserId);
            return criteria.SingleOrDefault();
        }

        public UserRelationshipType GetUserToBoxRelationShipType(long userId, long boxId)
        {
            var userBoxRel = GetUserBoxRelationship(userId, boxId);
            if (userBoxRel == null)
                return UserRelationshipType.None;
            return userBoxRel.UserRelationshipType;
        }
        public UserBoxRel GetUserBoxRelationship(long userId, long boxId)
        {
            return UnitOfWork.CurrentSession.QueryOver<UserBoxRel>().
            Where(w => w.User.Id == userId)
            .Where(w => w.Box.Id == boxId).SingleOrDefault();
        }

        public long GetItemsByUser(long userId)
        {
            return UnitOfWork.CurrentSession.Query<Item>()
                  .Where(w => w.User.Id == userId)
                  .Where(w => w.IsDeleted == false)
                  .Sum(s => s.Size);

        }



        public bool IsNotUsedCode(string code, long userId)
        {
            var user = UnitOfWork.CurrentSession.QueryOver<User>().
                Where(w => w.Id != userId)
               .Where(w => w.StudentId == code).SingleOrDefault();
            return user == null;
        }

        public void UpdateUserReputation(int reputation, long userid)
        {
            UnitOfWork.CurrentSession.GetNamedQuery("UpdateUserReputation")//.Get.CreateQuery(hqlUpdate)
                .SetInt64("reputation", reputation)
                .SetInt64("userid", userid)
                .ExecuteUpdate();
        }

        public void UpdateScore(int score, long userid)
        {
            UnitOfWork.CurrentSession.GetNamedQuery("UpdateUserScore")//.Get.CreateQuery(hqlUpdate)
                .SetInt64("Score", score)
                .SetInt64("userid", userid)
                .ExecuteUpdate();
        }

        //public void RegisterUserNotification(long userid, MobileOperatingSystem os)
        //{
        //    UnitOfWork.CurrentSession.GetNamedQuery("UpdateUserMobileDevice")
        //        .SetInt64("userid", userid)
        //        .SetEnum("mobileDevice", os)
        //        .ExecuteUpdate();
        //}

        public void UnsubscribeUserFromMail(IEnumerable<string> emails, EmailSend type)
        {
            var x = UnitOfWork.CurrentSession.GetNamedQuery("UpdateUserUnsubscribe")
                .SetEnum("emailSettings",type)
                .SetParameterList("userEmail", emails)
                .SetDateTime("dateTime", DateTime.UtcNow)
                .ExecuteUpdate();
            //TraceLog.WriteInfo($"updating {x} users in type {type}");

        }


        public UserLibraryRelationType GetUserToDepartmentRelationShipType(long userId, Guid departmentId)
        {

            var userBoxRel = UnitOfWork.CurrentSession.QueryOver<UserLibraryRel>()
                .Where(w => w.User.Id == userId)
                .Where(w => w.Library.Id == departmentId).SingleOrDefault();

            return userBoxRel?.UserType ?? UserLibraryRelationType.None;
        }
    }
}
