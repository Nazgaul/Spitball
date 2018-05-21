﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;

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

    public interface IQuestionRepository : IRepository<Question>
    {
        Task<QuestionDetailDto> GetQuestionDtoAsync(long id, CancellationToken token);
    }
}