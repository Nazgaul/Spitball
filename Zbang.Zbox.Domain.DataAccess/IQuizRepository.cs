using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IQuizRepository : IRepository<Quiz>
    {
        int ComputeAverage(long quizId);
        double ComputeStdevp(long quizId);
    }
}