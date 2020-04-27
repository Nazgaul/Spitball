using Cloudents.Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Cloudents.Query.Admin
{
    public class SessionPaymentsQueryV2 : IQueryAdmin<IEnumerable<PaymentDto>>
    {
        public SessionPaymentsQueryV2(string country)
        {
            Country = country;
        }
        public string? Country { get; }
        internal sealed class SessionPaymentsV2QueryHandler : IQueryHandler<SessionPaymentsQueryV2, IEnumerable<PaymentDto>>
        {
            private readonly IStatelessSession _session;

            public SessionPaymentsV2QueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<PaymentDto>> GetAsync(SessionPaymentsQueryV2 query, CancellationToken token)
            {
                var queryExpression = _session.Query<StudyRoomSessionUser>()
                    .Fetch(f => f.StudyRoomSession)
                    .ThenFetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Tutor)
                    .ThenFetch(f => f.User)
                    .Fetch(f => f.User)
                    .Where(w => w.Duration > StudyRoomSession.BillableStudyRoomSession)
                    .Where(w => w.Receipt == null);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    queryExpression = queryExpression.Where(w => w.StudyRoomSession.StudyRoom.Tutor.User.Country == query.Country);
                }

                return await queryExpression.Select(s => new PaymentDto()
                {
                    TutorId = s.StudyRoomSession.StudyRoom.Tutor.Id,
                    UserId = s.User.Id,
                    Price = s.TotalPrice,
                    Created = s.StudyRoomSession.Created,
                    StudyRoomSessionId = s.StudyRoomSession.Id,
                    UserName = s.User.Name,
                    IsPaymentKeyExists = s.User.PaymentExists == PaymentStatus.Done,
                    IsSellerKeyExists = s.StudyRoomSession.StudyRoom.Tutor.SellerKey != null,
                    TutorName = s.StudyRoomSession.StudyRoom.Tutor.User.Name,
                    _duration = s.Duration,
                    _realDuration = s.TutorApproveTime
                }).ToListAsync(token);
            }
        }
    }
}
