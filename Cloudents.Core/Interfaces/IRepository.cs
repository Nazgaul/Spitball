﻿using System.Collections.Generic;
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

        Task<User> GetRandomFictiveUserAsync(CancellationToken token);
        
        Task<decimal> UserBalanceAsync(long userId, CancellationToken token);
        Task<User> LoadAsync(object id, bool checkUserLocked, CancellationToken token);
    }

    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IList<Question>> GetAllQuestionsAsync(int page);
        Task<IList<Question>> GetOldQuestionsAsync(CancellationToken token);
        Task<Question> GetUserLastQuestionAsync(long userId, CancellationToken token);
    }

   

    public interface IUniversityRepository : IRepository<University>
    {
        [ItemCanBeNull]
        Task<University> GetUniversityByNameAsync(string name, 
            string country,
            CancellationToken token);

    }

    public interface IMailGunStudentRepository : IRepository<MailGunStudent>
    {
    }
}