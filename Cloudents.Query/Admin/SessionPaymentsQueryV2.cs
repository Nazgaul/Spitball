using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    public class SessionPaymentsQueryV2 : IQueryAdmin2<IEnumerable<PaymentDto>>
    {
        public SessionPaymentsQueryV2(Country? country)
        {
            Country = country;
        }
        public Country? Country { get; }
        internal sealed class SessionPaymentsV2QueryHandler : IQueryHandler<SessionPaymentsQueryV2, IEnumerable<PaymentDto>>
        {
            private readonly IStatelessSession _session;

            public SessionPaymentsV2QueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<PaymentDto>> GetAsync(SessionPaymentsQueryV2 query, CancellationToken token)
            {
                var queryExpression = _session.Query<StudyRoomPayment>()
                    .Fetch(f=>f.StudyRoomSessionUser)
                    //.Fetch(f => f.StudyRoomSession)
                    //.ThenFetch(f => f.StudyRoom)
                    .Fetch(f => f.Tutor)
                    .Fetch(f => f.User)
                    //.Fetch(f => f.User)
                    //.Where(w => w.Duration > StudyRoomSession.BillableStudyRoomSession)
                    .Where(w => w.Receipt == null);
                if (query.Country != null)
                {
                    queryExpression = queryExpression.Where(w => w.Tutor.User.SbCountry == query.Country);
                }

                return await queryExpression.Select(s => new PaymentDto()
                {
                    TutorId = s.Tutor.Id,
                    UserId = s.User.Id,
                    Price = s.TotalPrice,
                    Created = s.Created,
                    StudyRoomSessionId = s.Id,
                    UserName = s.User.Name,
                    IsPaymentKeyExists = s.User.PaymentExists == PaymentStatus.Done,
                    _sellerKey = s.Tutor.SellerKey,
                    TutorCountry = s.Tutor.User.SbCountry,
                    TutorName = s.Tutor.User.Name,
                    _duration = s.StudyRoomSessionUser!.Duration,
                    _realDuration = s.TutorApproveTime
                }).ToListAsync(token);
            }
        }
    }
}
