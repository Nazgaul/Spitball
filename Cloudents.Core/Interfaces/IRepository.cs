﻿using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        Task<object> AddAsync(T entity, CancellationToken token);
        Task AddAsync(IEnumerable<T> entities, CancellationToken token);

        Task<T> LoadAsync(object id, CancellationToken token);
        Task<T?> GetAsync(object id, CancellationToken token);

        T Load(object id);

        Task DeleteAsync(T entity, CancellationToken token);
        Task UpdateAsync(T entity, CancellationToken token);

        // Task FlushAsync(CancellationToken token);
    }

    public interface IFictiveUserRepository : IRepository<SystemUser>
    {
        Task<SystemUser> GetRandomFictiveUserAsync(string country, CancellationToken token);
    }


    public interface IRegularUserRepository : IRepository<User>
    {
        Task<decimal> UserBalanceAsync(long userId, CancellationToken token);
        Task<User> GetUserByEmailAsync(string userEmail, CancellationToken token);
        Task<IEnumerable<User>> GetExpiredCreditCardsAsync(CancellationToken token);
    }

    public interface IDocumentRepository : IRepository<Document>
    {
        Task UpdateNumberOfViews(long id, CancellationToken token);
        Task UpdateNumberOfDownloads(long id, CancellationToken token);
    }

    public interface IQuestionRepository : IRepository<Question>
    {
        Task<bool> GetSimilarQuestionAsync(string text, CancellationToken token);
    }



    public interface IAnswerRepository : IRepository<Answer>
    {
        Task<int> GetNumberOfPendingAnswer(long userId, CancellationToken token);
    }

    public interface IChatRoomRepository : IRepository<ChatRoom>
    {
        Task<ChatRoom?> GetChatRoomAsync(IEnumerable<long> usersId, CancellationToken token);

        Task<ChatRoom> GetOrAddChatRoomAsync(IList<long> userIds, CancellationToken token);
        Task<ChatRoom?> GetChatRoomAsync(string identifier, CancellationToken token);
        Task UpdateNonDayOldConversationToActiveAsync(CancellationToken token);
    }

    public interface ITutorRepository : IRepository<Tutor>
    {
        Task<IList<long>> GetTutorsByCourseAsync(string course, long userId, string country, CancellationToken token);
    }

    



    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<decimal> GetBalanceAsync(long userId, CancellationToken token);
    }

   
    public interface IReferUserTransactionRepository : IRepository<ReferUserTransaction>
    {
        Task<int> GetReferUserCountAsync(long userId, CancellationToken token);
    }
    public interface ICourseSubjectRepository : IRepository<CourseSubject>
    {
        Task<CourseSubject> GetCourseSubjectByName(string name, CancellationToken token);
    }

    public interface ILeadRepository : IRepository<Lead>
    {
        Task<bool> NeedToSendMoreTutorsAsync(long userId, CancellationToken token);
    }

    public interface IReadTutorRepository : IRepository<ReadTutor>
    {
        Task<ReadTutor?> GetReadTutorAsync(long userId, CancellationToken token);
    }

    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon> GetCouponAsync(string coupon, CancellationToken token);
    }

    public interface ITutorCalendarRepository : IRepository<TutorCalendar>
    {
        Task<IEnumerable<TutorCalendar>> GetTutorCalendarsAsync(long tutorId, CancellationToken token);
    }

    public interface ITutorHoursRepository : IRepository<TutorHours>
    {
        Task<IEnumerable<TutorHours>> GetTutorHoursAsync(long tutorId, CancellationToken token);
    }

    //public interface IUniversityRepository : IRepository<University>
    //{
    //    Task<University> GetUniversityByNameAndCountryAsync(string name, string country, CancellationToken token);
    //}

    public interface IAdminLanguageRepository : IRepository<AdminLanguage>
    {
        Task<AdminLanguage> GetLanguageByNameAsync(string name, CancellationToken token);
    }

    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetCoursesBySubjectIdAsync(long subjectId, CancellationToken token);
        Task MigrateCourseAsync(string courseToKeepId, string courseToRemoveId, CancellationToken token);
        Task RenameCourseAsync(string courseName, string newName, CancellationToken token);
    }
}