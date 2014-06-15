using System.Linq;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IQuestionRepository : IRepository<Comment>
    {
        IQueryable<CommentReplies> GetAnswers(Comment question);
    }
}
