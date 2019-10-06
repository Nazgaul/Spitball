﻿using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using NHibernate.Linq;

namespace Cloudents.Persistence.Repositories
{
    public class ReadTutorRepository : NHibernateRepository<ReadTutor>, IReadTutorRepository
    {
        public ReadTutorRepository(ISession session) : base(session)
        {

        }

        public async Task<ReadTutor> GetReadTutorAsync(long userId, CancellationToken token)
        {
            var tutorFutureValue = Session.Query<Tutor>()
                .Fetch(f => f.User)
                .ThenFetch(f => f.University)
                .Where(w => w.Id == userId && w.State == ItemState.Ok)
                .Select(s => new
                {
                    s.User.Id,
                    s.User.Name,
                    s.User.Image,
                    s.Bio,
                    UniversityName = s.User.University.Name,
                    s.Price,

                }).ToFutureValue();

            var coursesFuture = Session.Query<UserCourse>()
                .Fetch(f => f.Course)
                .ThenFetch(f => f.Subject)
                .Where(w => w.CanTeach && w.User.Id == userId)
                .Select(s => new
                {
                    CourseName = s.Course.Id,
                    SubjectName = s.Course.Subject.Name
                }).ToFuture();

            var reviewsFutureValue = Session.Query<TutorReview>()
                .Where(w => w.Tutor.Id == userId).GroupBy(g => g)
                .Select(s => new { count = s.Count(), average = s.Average(x => x.Rate) })
                .ToFutureValue();

            var lessonsFutureValue = Session.Query<StudyRoomSession>()
                .Fetch(f => f.StudyRoom)
                .Where(w => w.StudyRoom.Tutor.Id == userId && w.Duration > TimeSpan.FromMinutes(10))
                .GroupBy(g => g)
                .Select(s => s.Count())
                .ToFutureValue();


            var tutor = await tutorFutureValue.GetValueAsync(token);
            if (tutor is null)
            {
                return null;
            }

            var course = (await coursesFuture.GetEnumerableAsync(token)).ToList();
            var reviews = await reviewsFutureValue.GetValueAsync(token);
            var lessons = await lessonsFutureValue.GetValueAsync(token);



            var readTutor = new ReadTutor(tutor.Id, tutor.Name, tutor.Image,
                course.Select(s => s.SubjectName), course.Select(s => s.CourseName),
                tutor.Price, reviews?.average, reviews?.count ?? 0, tutor.Bio, tutor.UniversityName, lessons);
            //.SingleOrDefaultAsync(token);


            return readTutor;
        }
    }
}
