using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
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

        public IEnumerable<long> GetUsersToUpdate(long boxId, long notIncludeUserId)
        {
            QueryOver< UserBoxRel> userIds = QueryOver.Of<UserBoxRel>().Where(w => w.BoxId == boxId).Select(s => s.UserId);
            

            return UnitOfWork.CurrentSession.QueryOver<User>()
                .Where(w => w.IsRegisterUser)
                .And(w => w.UserType != UserType.Jared)
                .And(w => w.Id != notIncludeUserId)
                .WithSubquery.WhereProperty(x=>x.Id).In(userIds)
                .Select(s => s.Id).List<long>();

        }
        

        public void UpdateScore(int score, long userId)
        {
            UnitOfWork.CurrentSession.GetNamedQuery("UpdateUserScore")//.Get.CreateQuery(hqlUpdate)
                .SetInt64("Score", score)
                .SetInt64("userid", userId)
                .ExecuteUpdate();
        }


        public void UnsubscribeUserFromMail(IEnumerable<string> emails, EmailSend type)
        {
            UnitOfWork.CurrentSession.GetNamedQuery("UpdateUserUnsubscribe")
                .SetEnum("emailSettings", type)
                .SetParameterList("userEmail", emails)
                .SetDateTime("dateTime", DateTime.UtcNow)
                .ExecuteUpdate();

        }


        public UserLibraryRelationType GetUserToDepartmentRelationShipType(long userId, Guid departmentId)
        {

            var userBoxRel = UnitOfWork.CurrentSession.QueryOver<UserLibraryRel>()
                .Where(w => w.User.Id == userId)
                .Where(w => w.Library.Id == departmentId).SingleOrDefault();

            return userBoxRel?.UserType ?? UserLibraryRelationType.None;
        }

        public int LikesCount(long userId)
        {
            var fCommentLikes = UnitOfWork.CurrentSession.QueryOver<CommentLike>()
                .Where(w => w.User.Id == userId)
                .Select(Projections.RowCount())
                .FutureValue<int>();
            var fReplyLikes = UnitOfWork.CurrentSession.QueryOver<ReplyLike>()
               .Where(w => w.User.Id == userId)
               .Select(Projections.RowCount())
               .FutureValue<int>();
            var fItemLikes = UnitOfWork.CurrentSession.QueryOver<ItemRate>()
               .Where(w => w.User.Id == userId)
               .Select(Projections.RowCount())
               .FutureValue<int>();
            var fFlashcardLikes = UnitOfWork.CurrentSession.QueryOver<FlashcardLike>()
               .Where(w => w.User.Id == userId)
               .Select(Projections.RowCount())
               .FutureValue<int>();

            return fCommentLikes.Value + fReplyLikes.Value + fItemLikes.Value + fFlashcardLikes.Value;
        }

        public int ItemCount(long userId)
        {
            return UnitOfWork.CurrentSession.QueryOver<Item>()
                .Where(w => w.User.Id == userId).And(w => !w.IsDeleted)
                .Select(Projections.RowCount())
                .SingleOrDefault<int>();

        }


        public int QuizCount(long userId)
        {
            return UnitOfWork.CurrentSession.QueryOver<Quiz>()
               .Where(w => w.User.Id == userId).And(w => !w.IsDeleted).And(w => w.Publish)
               .Select(Projections.RowCount())
               .SingleOrDefault<int>();

        }

        public void UpdateUserFeedDetails(long userId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("UpdateUserFeed");
            query.SetInt64("userid", userId);
            query.ExecuteUpdate();
        }
    }
}
