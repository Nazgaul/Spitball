﻿using System;
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
    public interface IRepository<T>  where T : class
    {
       Task<object> AddAsync(T entity, CancellationToken token);

        //Task SaveOrUpdateAsync(T entity, CancellationToken token);
        //Task<T> LoadAsync(object id, CancellationToken token);
        Task<T> GetAsync(object id, CancellationToken token);

        Task DeleteAsync(T entity, CancellationToken token);
        Task UpdateAsync(T entity, CancellationToken token);
        Task AddOrUpdateAsync(T entity, CancellationToken token);

        //IQueryable<T> GetQueryable();
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
    }

    public interface IQuestionRepository : IRepository<Question>
    {
        [ItemCanBeNull]
        Task<QuestionDetailDto> GetQuestionDtoAsync(long id, CancellationToken token);

        Task<ResultWithFacetDto<QuestionDto>> GetQuestionsAsync(QuestionsQuery query, CancellationToken token);
    }

    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course> GetCourseAsync(long universityId, string courseName, CancellationToken token);
    }


    public interface IMailGunStudentRepository : IRepository<MailGunStudent>
    {
    }

}