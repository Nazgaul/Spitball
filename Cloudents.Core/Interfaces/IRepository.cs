using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IRepository<T>  where T : class
    {
       Task<object> AddAsync(T entity, CancellationToken token);
       Task AddAsync(IEnumerable<T> entities, CancellationToken token);

        Task<T> LoadAsync(object id, CancellationToken token);
        Task<T> GetAsync(object id, CancellationToken token);

        T Load(object id);

        Task DeleteAsync(T entity, CancellationToken token);
        Task UpdateAsync(T entity, CancellationToken token);
    }


    public interface IUserRepository : IRepository<User>
    {
        Task<IList<User>> GetAllUsersAsync(CancellationToken token);

        Task<decimal> UserEarnedBalanceAsync(long userId, CancellationToken token);
    }

    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IList<Question>> GetAllQuestionsAsync();
    }

    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course> GetCourseAsync(long universityId, string courseName, CancellationToken token);
    }




    public interface IMailGunStudentRepository : IRepository<MailGunStudent>
    {
    }
}