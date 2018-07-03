using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
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

    public interface IQuestionSubjectRepository : IRepository<QuestionSubject>
    {
        Task<IEnumerable<QuestionSubjectDto>> GetAllSubjectAsync(CancellationToken token);
    }

    public interface IUserRepository : IRepository<User>
    {
        [ItemCanBeNull]
        Task<ProfileDto> GetUserProfileAsync(long id, CancellationToken token);

        Task<User> GetUserByExpressionAsync(Expression<Func<User, bool>> expression, CancellationToken token);

        Task<UserAccountDto> GetUserDetailAsync(long id, CancellationToken token);

        Task<IList<User>> GetAllUsersAsync(CancellationToken token);

        Task<decimal> UserEarnedBalanceAsync(long userId, CancellationToken token);
    }

    public interface IQuestionRepository : IRepository<Question>
    {
        //[ItemCanBeNull]
        //Task<QuestionDetailDto> GetQuestionDtoAsync(long id, CancellationToken token);

        //Task<ResultWithFacetDto<QuestionDto>> GetQuestionsAsync(QuestionsQuery query, CancellationToken token);

        Task<IList<Question>> GetAllQuestionsAsync();
    }

   

    

    public interface IMailGunStudentRepository : IRepository<MailGunStudent>
    {
    }
}