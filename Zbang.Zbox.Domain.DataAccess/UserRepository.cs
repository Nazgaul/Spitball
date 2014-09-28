using System;
using System.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Enums;
using System.Collections.Generic;

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

        public User GetUserByMembershipId(Guid membershipId)
        {
            var criteria = UnitOfWork.CurrentSession.QueryOver<User>();
            criteria.Where(w => w.MembershipId == membershipId);

            return criteria.SingleOrDefault();
        }
        public User GetUserByFacebookId(long facebookUserId)
        {
            var criteria = UnitOfWork.CurrentSession.QueryOver<User>();
            criteria.Where(w => w.FacebookId == facebookUserId);

            return criteria.SingleOrDefault();
        }

        public UserRelationshipType GetUserToBoxRelationShipTypeWithInvite(long userId, long boxId)
        {
            var fUserBoxRel = UnitOfWork.CurrentSession.QueryOver<UserBoxRel>()
                                        .Where(w => w.User.Id == userId)
                                        .Where(w => w.Box.Id == boxId).Future<UserBoxRel>();
            //.SingleOrDefault();
            var fUserInvite = UnitOfWork.CurrentSession.QueryOver<Invite>()
                                        .Where(w => w.Recepient.Id == userId)
                                        .Where(w => w.Box.Id == boxId)
                                        .Where(w => w.IsActive).Future<Invite>();

            var userBoxRel = fUserBoxRel.SingleOrDefault();


            if (userBoxRel == null)
            {
                if (fUserInvite.SingleOrDefault() == null)
                {
                    return UserRelationshipType.None;
                }
                return UserRelationshipType.Invite;

            }
            return userBoxRel.UserRelationshipType;
        }
        public UserRelationshipType GetUserToBoxRelationShipType(long userId, long boxId)
        {
            var userBoxRel = GetUserBoxRelationship(userId, boxId);
            if (userBoxRel == null)
                return UserRelationshipType.None;
            return userBoxRel.UserRelationshipType;
        }
        private UserBoxRel GetUserBoxRelationship(long userId, long boxId)
        {
            return UnitOfWork.CurrentSession.QueryOver<UserBoxRel>().
            Where(w => w.User.Id == userId)
            .Where(w => w.Box.Id == boxId).SingleOrDefault();
        }

        public IEnumerable<Item> GetItemsByUser(long userId)
        {
            var x = UnitOfWork.CurrentSession.QueryOver<Item>().
                         Where(w => w.Uploader.Id == userId).Where(w => w.IsDeleted == false)

                            //.Select(NHibernate.Criterion.Projections.Sum<Item>(s=>s.Size)).SingleOrDefault();
                        .List<Item>();
            return x;

        }

        public User GetOwnerByBoxId(long boxId)
        {
            return UnitOfWork.CurrentSession.QueryOver<UserBoxRel>().
                Where(w => w.Box.Id == boxId).Where(w => w.UserRelationshipType == UserRelationshipType.Owner).SingleOrDefault().User;
        }

        public bool IsNotUsedCode(string code, long userId)
        {
            var user = UnitOfWork.CurrentSession.QueryOver<User>().
                Where(w => w.Id != userId)
               .Where(w => w.Code == code).SingleOrDefault();
            return user == null;
        }
    }
}
