
using System;
using System.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ItemRepository : NHibernateRepository<Item>, IItemRepository
    {
        public bool CheckFileNameExists(string fileName, Box box)
        {
            var file = UnitOfWork.CurrentSession.QueryOver<File>()
                // ReSharper disable once PossibleUnintendedReferenceComparison nhibernate issue
                .Where(w => w.Box == box)
                .And(w => w.Name == fileName)
                .SingleOrDefault();
            return file != null;
        }

        public Comment GetPreviousCommentId(Box box)
        {
            var questions = UnitOfWork.CurrentSession.QueryOver<Item>()
                .Where(w => w.DateTimeUser.CreationTime > DateTime.UtcNow.AddHours(-1))
                .Select(s => s.Question).List<Comment>();

            var questions2 = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                .Where(w => w.DateTimeUser.CreationTime > DateTime.UtcNow.AddHours(-1))
                .Select(s => s.Comment).List<Comment>();

            return questions.Union(questions2)
                .Where(w => w != null)
                .Where(w => w.Text == null)
                .OrderByDescending(o => o.DateTimeUser.CreationTime)
                .FirstOrDefault();
        }
    }
}
