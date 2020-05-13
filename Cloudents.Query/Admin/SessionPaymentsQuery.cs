﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Cloudents.Query.Admin
{
    public class SessionPaymentsQuery : IQueryAdmin<IEnumerable<PaymentDto>>
    {
        [Obsolete]
        public SessionPaymentsQuery(string country)
        {
            Country = country;
        }
        public string? Country { get; }
        internal sealed class PaymentsQueryHandler : IQueryHandler<SessionPaymentsQuery, IEnumerable<PaymentDto>>
        {
            private readonly IStatelessSession _session;

            public PaymentsQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<PaymentDto>> GetAsync(SessionPaymentsQuery query, CancellationToken token)
            {
                StudyRoomSession? studyRoomSessionAlias = null!;
                StudyRoom? studyRoomAlias = null!;
                Core.Entities.Tutor? tutorAlias = null!;
                StudyRoomUser? studyRoomUserAlias = null!;
                User? studentAlias = null!;
                User? tutorUserAlias = null!;

                PaymentDto? resultDto = null!;



                var res = _session.QueryOver(() => studyRoomSessionAlias)
                    .JoinAlias(x => x.StudyRoom, () => studyRoomAlias)
                    .JoinEntityAlias(() => tutorAlias, () => studyRoomAlias.Tutor.Id == tutorAlias.Id)
                    .JoinEntityAlias(() => studyRoomUserAlias,
                        () => studyRoomAlias.Id == studyRoomUserAlias.Room.Id &&
                              tutorAlias.Id != studyRoomUserAlias.User.Id)
                    .JoinEntityAlias(() => studentAlias, () => studyRoomUserAlias.User.Id == studentAlias.Id)
                    .JoinEntityAlias(() => tutorUserAlias, () => tutorUserAlias.Id == tutorAlias.User.Id)
                    .Where(w => w.Receipt == null)
                    .Where(w => w.StudyRoomVersion.Coalesce(0) != StudyRoomSession.StudyRoomNewVersion)
                    .Where(w => w.Duration!.Value > StudyRoomSession.BillableStudyRoomSession);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    res = res.Where(w => tutorUserAlias.Country == query.Country);
                }

                return await res.SelectList(sl =>
                        sl.Select(s => s.Id).WithAlias(() => resultDto.StudyRoomSessionId)
                            .Select(s => s.Price).WithAlias(() => resultDto.Price)
                            //.Select(Projections.Conditional(
                            //    Restrictions.IsNull(Projections.Property(() => tutorAlias.SellerKey)),
                            //    Projections.Constant(false),
                            //    Projections.Constant(true)
                            //)).WithAlias(() => resultDto._sellerKey)
                            .Select(Projections.Conditional(
                                Restrictions.Eq(Projections.Property(() => studentAlias.PaymentExists), PaymentStatus.None),
                                Projections.Constant(false),
                                Projections.Constant(true)
                            )).WithAlias(() => resultDto.IsPaymentKeyExists)
                            .Select(() => tutorAlias.SellerKey).WithAlias(() => resultDto._sellerKey)
                            .Select(() => tutorUserAlias.SbCountry).WithAlias(() => resultDto.TutorCountry)
                            .Select(() => tutorAlias.Id).WithAlias(() => resultDto.TutorId)
                            .Select(() => tutorUserAlias.Name).WithAlias(() => resultDto.TutorName)
                            .Select(() => studentAlias.Id).WithAlias(() => resultDto.UserId)
                            .Select(() => studentAlias.Name).WithAlias(() => resultDto.UserName)
                            .Select(s => s.Created).WithAlias(() => resultDto.Created)
                            .Select(s => s.Duration!.Value).WithAlias(() => resultDto._duration)
                            .Select(s => s.RealDuration).WithAlias(() => resultDto._realDuration)

                     ).TransformUsing(Transformers.AliasToBean<PaymentDto>())
                     .ListAsync<PaymentDto>(token);


            }
        }
    }
}