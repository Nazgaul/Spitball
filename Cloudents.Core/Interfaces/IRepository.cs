using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Query;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IRepository<T> /*: IDisposable*/ where T : class
    {
        Task<object> SaveAsync(T entity, CancellationToken token);
        Task<T> LoadAsync(object id, CancellationToken token);
        Task<T> GetAsync(object id, CancellationToken token);

        IQueryable<T> GetQueryable();
    }

    public interface IQuestionSubjectRepository : IRepository<QuestionSubject>
    {
        Task<IEnumerable<QuestionSubjectDto>> GetAllSubjectAsync(CancellationToken token);
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<ProfileDto> GetUserProfileAsync(long id, CancellationToken token);
        Task<User> GetUserByExpressionAsync(Expression<Func<User, bool>> expression, CancellationToken token);
    }

    public interface IQuestionRepository : IRepository<Question>
    {
        [ItemCanBeNull]
        Task<QuestionDetailDto> GetQuestionDtoAsync(long id, CancellationToken token);

        Task<ResultWithFacetDto<QuestionDto>> GetQuestionsAsync(QuestionsQuery query, CancellationToken token);
    }
}