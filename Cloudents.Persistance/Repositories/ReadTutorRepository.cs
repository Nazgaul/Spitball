using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Persistence.Repositories
{
    public class ReadTutorRepository : NHibernateRepository<ReadTutor>, IReadTutorRepository
    {
        public ReadTutorRepository(ISession session) : base(session)
        {
        }

        public Task AddOrUpdateAsync(ReadTutor entity, CancellationToken token)
        {
            return Session.SaveOrUpdateAsync(entity, token);
        }

        public async Task<ReadTutor?> GetReadTutorAsync(long userId, CancellationToken token)
        {
            var tutorFutureValue = Session.Query<Tutor>()
                .Fetch(f => f.User)
                .Where(w => w.Id == userId)
                .Select(s => new
                {
                    s.User.Id,
                    s.User.Name,
                    s.User.ImageName,
                    // s.User.Image,
                    Bio = s.Paragraph2,
                    s.User.SbCountry,
                    s.User.Country,
                    Description = s.Title,
                    s.SubscriptionPrice,
                    s.State
                }).ToFutureValue();

            var coursesFuture = Session.Query<Course>()
                .Where(w =>w.Tutor.Id == userId && w.State == ItemState.Ok)
                .Select(s => new
                {
                    CourseName = s.Name,
                }).ToFuture();

            var query = from e in Session.Query<TutorReview>()
                        where e.Tutor.Id == userId
                        group e by 1 into grp
                        select new
                        {
                            Count = grp.Count(),
                            //Average is doing issue - doing average in c# 
                            Sum = grp.Sum(x => (float?)x.Rate)
                        };
            var reviewsFutureValue = query.ToFutureValue();


            var lessonsFutureValue = Session.Query<StudyRoomSession>()
.Fetch(f => f.StudyRoom)
//.Where(w => w.StudyRoom.Tutor.Id == userId && w.Duration > StudyRoomSession.BillableStudyRoomSession)
.GroupBy(g => 1)
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

            var count = reviews?.Count ?? 0;
            var average = 0f;
            if (count > 0)
            {
                average = (reviews?.Sum ?? 0) / count;
            }

            var readTutor = new ReadTutor(tutor.Id, tutor.Name, tutor.ImageName,
                course.Select(s => s.CourseName).ToList(),
                 average, count, tutor.Bio,
                lessons, tutor.SbCountry, tutor.SubscriptionPrice, tutor.Description, tutor.State);


            return readTutor;
        }
    }
}
