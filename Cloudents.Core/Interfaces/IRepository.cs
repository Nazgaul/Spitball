using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

       // Task FlushAsync(CancellationToken token);
    }


    public interface IUserRepository : IRepository<User>
    {
        Task<decimal> UserEarnedBalanceAsync(long userId, CancellationToken token);

        Task<User> GetRandomFictiveUserAsync(string country, CancellationToken token);
        
        Task<decimal> UserBalanceAsync(long userId, CancellationToken token);
        Task<User> LoadAsync(object id, bool checkUserLocked, CancellationToken token);
        Task UpdateUsersBalance(CancellationToken token);
    }

    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course> GetOrAddAsync(string name, CancellationToken token);
    }

    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag> GetOrAddAsync(string name, CancellationToken token);
    }

    public interface IDocumentRepository : IRepository<Document>
    {
        Task UpdateNumberOfViews(long id, CancellationToken token);
        Task UpdateNumberOfDownloads(long id, CancellationToken token);
    }

    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IList<Question>> GetAllQuestionsAsync(int page);
        Task<IList<Question>> GetOldQuestionsAsync(CancellationToken token);
        Task<Question> GetUserLastQuestionAsync(long userId, CancellationToken token);
        Task<bool> GetSimilarQuestionAsync(string text, CancellationToken token);
    }

   

    public interface IAnswerRepository : IRepository<Answer>
    {
        Task<int> GetNumberOfPendingAnswer(long userId, CancellationToken token);
    }

    public interface IUniversityRepository : IRepository<University>
    {
        [ItemCanBeNull]
        Task<University> GetUniversityByNameAsync(string name, 
            string country,
            CancellationToken token);

    }

   
}