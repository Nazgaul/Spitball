using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class AnswerRatingRepository : NHibernateRepository<AnswerRating>, IAnswerRatingRepository
    {
        public AnswerRating GetAnswerRating(long userId, Guid answerId)
        {
            var query = UnitOfWork.CurrentSession.QueryOver<AnswerRating>();
            query.Where(b => b.Answer.Id == answerId);
            query.Where(b => b.User.Id == userId);
            return query.SingleOrDefault<AnswerRating>();
        }
    }
}
