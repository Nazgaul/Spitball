
using System;
using System.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ItemRepository : NHibernateRepository<Item>, IItemRepository
    {
        public bool CheckFileNameExists(string fileName, long boxId)
        {
            var file = UnitOfWork.CurrentSession.QueryOver<File>()
                // ReSharper disable once PossibleUnintendedReferenceComparison nhibernate issue
                .Where(w => w.Box.Id == boxId)
                .And(w => w.Name == fileName)
                .And(w => w.IsDeleted == false)
                .SingleOrDefault();
            return file != null;
        }

        public Comment GetPreviousCommentId(long boxId, long userId)
        {
            //UnitOfWork.CurrentSession.QueryOver<Comment>()
            //    .Where(w => w.FeedType == Infrastructure.Enums.FeedType.AddedItems)
            //    .And(w => w.Box.Id == boxId)
            //    .And(w => w.User.Id == userId)

            var questions = UnitOfWork.CurrentSession.QueryOver<Item>()
                .Where(w => w.DateTimeUser.CreationTime > DateTime.UtcNow.AddHours(-1))
                .And(w => w.Box.Id == boxId)
                .And(w => w.User.Id == userId)
                .And(w => w.IsDeleted == false)
                .Select(s => s.Comment).Future<Comment>();

            var questions2 = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                .Where(w => w.DateTimeUser.CreationTime > DateTime.UtcNow.AddHours(-1))
                .And(w => w.Box.Id == boxId)
                .And(w => w.User.Id == userId)
                .Select(s => s.Comment).Future<Comment>();

            return questions.Union(questions2)
                .Where(w => w != null)
                //.Where(w => w.Text == null)
                .Where(w => w.FeedType == Infrastructure.Enums.FeedType.AddedItems)
                .OrderByDescending(o => o.DateTimeUser.CreationTime)
                .FirstOrDefault();
        }
    }
}
