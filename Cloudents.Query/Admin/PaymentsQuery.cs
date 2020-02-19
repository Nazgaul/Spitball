using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class PaymentsQuery : IQueryAdmin<IEnumerable<PaymentDto>>
    {
        public PaymentsQuery(string country)
        {
            Country = country;
        }
        public string Country { get; }
        internal sealed class PaymentsQueryHandler : IQueryHandler<PaymentsQuery, IEnumerable<PaymentDto>>
        {
            private readonly IStatelessSession _session;

            public PaymentsQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<PaymentDto>> GetAsync(PaymentsQuery query, CancellationToken token)
            {
                StudyRoomSession studyRoomSessionAlias = null;
                StudyRoom studyRoomAlias = null;
                Core.Entities.Tutor tutorAlias = null;
                StudyRoomUser studyRoomUserAlias = null;
                User studentAlias = null;
                User tutorUserAlias = null;

                PaymentDto resultDto = null;

                var res = _session.QueryOver(() => studyRoomSessionAlias)
                            .JoinAlias(x => x.StudyRoom, () => studyRoomAlias)
                            .JoinEntityAlias(() => tutorAlias, () => studyRoomAlias.Tutor.Id == tutorAlias.Id)
                            .JoinEntityAlias(() => studyRoomUserAlias, 
                                () => studyRoomAlias.Id == studyRoomUserAlias.Room.Id &&
                                    tutorAlias.Id != studyRoomUserAlias.User.Id)
                            .JoinEntityAlias(() => studentAlias, () => studyRoomUserAlias.User.Id == studentAlias.Id)
                            .JoinEntityAlias(() => tutorUserAlias, () => tutorUserAlias.Id == tutorAlias.User.Id)
                            .Where(w => w.Receipt == null)
                            .Where(w => w.Duration.Value > TimeSpan.FromMinutes(10));
                if (!string.IsNullOrEmpty(query.Country))
                {
                    res = res.Where(w => studentAlias.Country == query.Country || tutorUserAlias.Country == query.Country);
                }

                return await res.SelectList(sl => 
                sl.Select(s => s.Id).WithAlias(() => resultDto.StudyRoomSessionId)
                .Select(s => s.Price).WithAlias(() => resultDto.Price)
                .Select(Projections.Conditional(
                    Restrictions.IsNull(Projections.Property(() => tutorAlias.SellerKey)),
                    Projections.Constant(false),
                    Projections.Constant(true)
                    )).WithAlias(() => resultDto.IsSellerKeyExists)
                .Select(Projections.Conditional(
                    Restrictions.Eq(Projections.Property(() => studentAlias.PaymentExists), PaymentStatus.None),
                    Projections.Constant(false),
                    Projections.Constant(true)
                    )).WithAlias(() => resultDto.IsPaymentKeyExists)
                .Select(s => tutorAlias.Id).WithAlias(() => resultDto.TutorId)
                .Select(s => tutorUserAlias.Name).WithAlias(() => resultDto.TutorName)
                .Select(s => studentAlias.Id).WithAlias(() => resultDto.UserId)
                .Select(s => studentAlias.Name).WithAlias(() => resultDto.UserName)
                .Select(s => s.Created).WithAlias(() => resultDto.Created)
                .Select(s => s.Duration.Value).WithAlias(() => resultDto.Duration)
                ).TransformUsing(Transformers.AliasToBean<PaymentDto>())
                    .ListAsync<PaymentDto>();
            }
        }
    }
}
