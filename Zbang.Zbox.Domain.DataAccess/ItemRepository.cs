
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
                .Where(w => w.Box.Id == boxId)
                .And(w => w.Name == fileName)
                .And(w => w.IsDeleted == false)
                .SingleOrDefault();
            return file != null;
        }

        public Comment GetPreviousCommentId(long boxId, long userId)
        {

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
            var question3 = UnitOfWork.CurrentSession.QueryOver<FlashcardMeta>()
                .Where(w => w.DateTimeUser.CreationTime > DateTime.UtcNow.AddHours(-1))
                .And(w => w.Box.Id == boxId)
                .And(w => w.User.Id == userId)
                .Select(s => s.Comment).Future<Comment>();

            return questions.Union(questions2).Union(question3)
                .Where(w => w != null)
                //.Where(w => w.Text == null)
                .Where(w => w.FeedType == Infrastructure.Enums.FeedType.AddedItems)
                .OrderByDescending(o => o.DateTimeUser.CreationTime)
                .FirstOrDefault();
        }
    }
}
