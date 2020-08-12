using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Courses
{
    public class CourseByIdEditQuery : IQuery<CourseDetailEditDto?>
    {
        private long Id { get; }

        private long UserId { get; }

        public CourseByIdEditQuery(long id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        internal sealed class CourseByIdQueryHandler : IQueryHandler<CourseByIdEditQuery, CourseDetailEditDto?>
        {
            private readonly IStatelessSession _statelessSession;

            public CourseByIdQueryHandler(IStatelessSession repository)
            {
                _statelessSession = repository;
            }

            public async Task<CourseDetailEditDto?> GetAsync(CourseByIdEditQuery query, CancellationToken token)
            {
                var futureCourse = _statelessSession.Query<Course>()
                    .Where(w => w.Id == query.Id && w.Tutor.Id == query.UserId)
                    .Select(s => new CourseDetailEditDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        SubscriptionPrice = s.SubscriptionPrice,
                        Description = s.Description,
                        Visible = s.State == ItemState.Ok,
                        Version = s.Version
                    }).ToFutureValue();


                var futureStudyRoom = _statelessSession.Query<BroadCastStudyRoom>()
                    .Where(w => w.Course.Id == query.Id)
                    .OrderBy(o => o.BroadcastTime)
                    .Select(
                        s2 => new CourseEditStudyRoomDto()
                        {
                            DateTime = s2.BroadcastTime,
                            Name = s2.Description
                        }).ToFuture();


                var futureDocuments = _statelessSession.Query<Document>()
                    .Where(w => w.Course.Id == query.Id)
                    .OrderBy(o => o.Position)
                    .Select(s2 =>
                    new CourseEditDocumentDto()
                    {
                        Title = s2.Name,
                        Visible = s2.Status.State == ItemState.Ok,
                        Id = s2.Id
                    }).ToFuture();



                var futureCoupon = _statelessSession.Query<Coupon>()
                    .Where(w => w.Course.Id == query.Id)
                    .Select(s => new CourseEditCouponDto
                    {
                        Id = s.Id,
                        Value = s.Value,
                        Code = s.Code,
                        CouponType = s.CouponType
                    })
                    .ToFuture();


                var result = await futureCourse.GetValueAsync(token);

                if (result is null)
                {
                    return null;
                }

                result.StudyRooms = await futureStudyRoom.GetEnumerableAsync(token);
                result.Documents = await futureDocuments.GetEnumerableAsync(token);
                result.Coupons = await futureCoupon.GetEnumerableAsync(token);

                return result;
            }
        }
    }
}