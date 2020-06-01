﻿using System;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Enum;

namespace Cloudents.Query.Users
{
    public class UserAccountQuery : IQuery<UserAccountDto?>
    {
        public UserAccountQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }


        internal sealed class UserAccountDataQueryHandler : IQueryHandler<UserAccountQuery, UserAccountDto?>
        {
            private readonly IStatelessSession _session;

            public UserAccountDataQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<UserAccountDto?> GetAsync(UserAccountQuery query, CancellationToken token)
            {
                var userFuture = _session.Query<User>().Fetch(f=>f.Tutor)
                    .Where(w => w.Id == query.Id)
                    .Where(w => w.LockoutEnd == null || w.LockoutEnd < DateTime.UtcNow)
                    .Select(s => new UserAccountDto
                    {
                        Id = s.Id,
                        Balance = s.Transactions.Balance,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Image = s.ImageName,
                        Email = s.Email,
                        country = s.SbCountry,
                        IsTutor =  s.Tutor!.State,
                        TutorSubscription = s.Tutor.SubscriptionPrice != null,
                        Price =  s.Tutor.Price.Price,
                        _needPayment = s.PaymentExists.GetValueOrDefault(PaymentStatus.None) == PaymentStatus.None
                    }).ToFutureValue();
                
                var pendingSessionsPaymentsFuture = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .Where(w => w.StudyRoom.Tutor.Id == query.Id)
                    .Where(w => w.StudyRoomVersion.GetValueOrDefault() == 0)
                    .Where(w => w.Duration > StudyRoomSession.BillableStudyRoomSession
                                && w.RealDuration == null && w.Receipt == null)
                    .GroupBy(g => 1)
                    .Select(s => s.Count())
                    .ToFutureValue();

                var newPendingSessionPayment = _session.Query<StudyRoomSessionUser>()
                    .Fetch(f => f.StudyRoomPayment)
                    .Where(w => w.StudyRoomPayment.Tutor.Id == query.Id
                                && w.Duration > StudyRoomSession.BillableStudyRoomSession
                                && w.StudyRoomPayment.TutorApproveTime == null)
                    .GroupBy(g => 1)
                    .Select(s => s.Count())
                    .ToFutureValue();

                var haveDocsFuture = _session.Query<Document>()
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var haveQuestionsFuture = _session.Query<Question>()
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var haveDocsWithPriceFuture = _session.Query<Document>()
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok && w.DocumentPrice.Price > 0)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var purchasedDocsFuture = _session.Query<DocumentTransaction>()
                    .Fetch(f => f.User)
                    .Fetch(f => f.Document)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Document.Status.State == ItemState.Ok)
                    .Where(w => w.Type == TransactionType.Spent)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var buyPointsFuture = _session.Query<BuyPointsTransaction>()
                    .Where(w => w.User.Id == query.Id)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var purchasedSessionsFuture = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Users)
                    .Where(w => w.StudyRoom.Users.Select(s => s.User.Id).Any(a => a == query.Id) && query.Id != w.StudyRoom.Tutor.Id)
                    .Where(w => w.Ended != null)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();



                var isSoldDocumentFuture = _session.Query<DocumentTransaction>()
                    .Fetch(f => f.User)
                    .Fetch(f => f.Document)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Type == TransactionType.Earned)
                    .Where(w => w.Document.Status.State == ItemState.Ok)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var isSoldSessionFuture = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Users)
                    .Where(w => w.StudyRoom.Tutor.Id == query.Id && w.Ended != null)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var haveFollowersFuture = _session.Query<Follow>()
                    .Where(w => w.User.Id == query.Id)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var result = await userFuture.GetValueAsync(token);
                if (result is null)
                {
                    return null;
                }

                result.HaveContent = (await haveDocsFuture.GetEnumerableAsync(token)).Any()
                                     || (await haveQuestionsFuture.GetEnumerableAsync(token)).Any();

                result.HaveDocsWithPrice = (await haveDocsWithPriceFuture.GetEnumerableAsync(token)).Any();

                result.IsPurchased = (await purchasedDocsFuture.GetEnumerableAsync(token)).Any()
                                     || (await purchasedSessionsFuture.GetEnumerableAsync(token)).Any()
                                     || (await buyPointsFuture.GetEnumerableAsync(token)).Any();


                result.IsSold = (await isSoldDocumentFuture.GetEnumerableAsync(token)).Any()
                                || (await isSoldSessionFuture.GetEnumerableAsync(token)).Any();

                result.HaveFollowers = (await haveFollowersFuture.GetEnumerableAsync(token)).Any();
                result.PendingSessionsPayments =
                     pendingSessionsPaymentsFuture.Value + newPendingSessionPayment.Value;
                return result;
            }
        }
    }
}