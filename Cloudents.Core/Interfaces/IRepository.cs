using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IRepository<T> where T : class
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

    public interface IFictiveUserRepository : IRepository<SystemUser>
    {
        Task<SystemUser> GetRandomFictiveUserAsync(string country, CancellationToken token);

    }


    public interface IRegularUserRepository : IRepository<RegularUser>
    {
        Task<decimal> UserCashableBalanceAsync(long userId, CancellationToken token);


        //Task<decimal> UserBalanceAsync(long userId, CancellationToken token);
        //Task<RegularUser> LoadAsync(object id, bool checkUserLocked, CancellationToken token);
        // Task UpdateUsersBalance(CancellationToken token);
    }

    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course> GetOrAddAsync(string name, CancellationToken token);
    }

    public interface IVoteRepository : IRepository<Vote>
    {
        Task<Vote> GetVoteQuestionAsync(long userId, long questionId, CancellationToken token);
        Task<Vote> GetVoteDocumentAsync(long userId, long documentId, CancellationToken token);
        Task<Vote> GetVoteAnswerAsync(long userId, Guid answerId, CancellationToken token);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns>the user answer otherwise null</returns>
        Task<Answer> GetUserAnswerInQuestion(long questionId, long userId, CancellationToken token);
        Task<int> GetNumberOfPendingAnswer(long userId, CancellationToken token);
    }

    public interface IUniversityRepository : IRepository<University>
    {
        [ItemCanBeNull]
        Task<University> GetUniversityByNameAsync(string name,
            string country,
            CancellationToken token);

    }
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<decimal> GetUserScoreAsync(long userId, CancellationToken token);
        Task<decimal> GetBalanceAsync(long userId, CancellationToken token);
        Task<TransactionActionType> GetFirstCourseTransaction(long userId, CancellationToken token);

    }

    public interface IStatsRepository : IRepository<Stats>
    {
        Task UpdateTableAsync(CancellationToken token);
    }

}