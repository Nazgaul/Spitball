﻿
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
                .And(w => w.IsDeleted == false)
                .SingleOrDefault();
            return file != null;
        }

        public Comment GetPreviousCommentId(Box box, User user)
        {
            var questions = UnitOfWork.CurrentSession.QueryOver<Item>()
                .Where(w => w.DateTimeUser.CreationTime > DateTime.UtcNow.AddHours(-1))
                // ReSharper disable once PossibleUnintendedReferenceComparison nhibernate issue
                .And(w => w.Box == box)
                // ReSharper disable once PossibleUnintendedReferenceComparison nhibernate issue
                .And(w => w.Uploader == user)
                .Select(s => s.Comment).Future<Comment>();

            var questions2 = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                .Where(w => w.DateTimeUser.CreationTime > DateTime.UtcNow.AddHours(-1))
                // ReSharper disable once PossibleUnintendedReferenceComparison nhibernate issue
                .And(w => w.Box == box)
                // ReSharper disable once PossibleUnintendedReferenceComparison nhibernate issue
                .And(w => w.Owner == user)
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
