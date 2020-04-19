using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.DTOs.Users;

namespace Cloudents.Query.Users
{
    public class UserPurchasesByIdQuery : IQuery<IEnumerable<UserPurchaseDto>>
    {
        public UserPurchasesByIdQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }

        internal sealed class UserPurchasesByIdQueryHandler : IQueryHandler<UserPurchasesByIdQuery, IEnumerable<UserPurchaseDto>>
        {
            private readonly IStatelessSession _session;
            private readonly IUrlBuilder _urlBuilder;

            public UserPurchasesByIdQueryHandler(IStatelessSession session, IUrlBuilder urlBuilder)
            {
                _session = session;
                _urlBuilder = urlBuilder;
            }

            public async Task<IEnumerable<UserPurchaseDto>> GetAsync(UserPurchasesByIdQuery query, CancellationToken token)
            {

                //TODO: need to return Document with state = archive
                var documentFuture = _session.Query<DocumentTransaction>()
                    .WithOptions(w => w.SetComment(nameof(UserPurchasesByIdQuery)))
                    .Fetch(f => f.User)
                    .Fetch(f => f.Document)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Document.Status.State == Core.Enum.ItemState.Ok)
                    .Where(w => w.Type == Core.Enum.TransactionType.Spent)
                    .Select(s => new PurchasedDocumentDto()
                    {
                        Id = s.Document.Id,
                        Name = s.Document.Name,
                        Course = s.Document.Course2.CardDisplay,
                        Type = s.Document.DocumentType != null ?
                            (ContentType)s.Document.DocumentType :
                            ContentType.Document,
                        Date = s.Created,
                        Price = s.Price

                    }).ToFuture<UserPurchaseDto>();

                var sessionFuture = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Users)
                    .Where(w => w.StudyRoom.Users.Select(s => s.User.Id).Any(a => a == query.Id)
                                && query.Id != w.StudyRoom.Tutor.Id)
                    .Where(w => w.Ended != null)
                    .Where(w => w.StudyRoomVersion.GetValueOrDefault(0) == 0)
                    .Where(w=>w.Duration > StudyRoomSession.BillableStudyRoomSession)
                    .Select(s => new PurchasedSessionDto()
                    {
                        Date = s.Created,
                        Price = s.Price.GetValueOrDefault(),
                        Duration = s.Duration,
                        TutorName = s.StudyRoom.Tutor.User.Name,
                        TutorId = s.StudyRoom.Tutor.Id,
                        TutorImage = _urlBuilder.BuildUserImageEndpoint(s.StudyRoom.Tutor.Id, s.StudyRoom.Tutor.User.ImageName, s.StudyRoom.Tutor.User.Name, null)
                    }).ToFuture<UserPurchaseDto>();


                var newSessionFuture = _session.Query<StudyRoomSessionUser>()
                    .Fetch(f=>f.StudyRoomSession)
                    .ThenFetch(f=>f.StudyRoom)
                    .Where(w=>w.User.Id == query.Id && w.StudyRoomSession.Ended != null)
                    .Where(w=>w.Duration > StudyRoomSession.BillableStudyRoomSession)
                    .Select(s => new PurchasedSessionDto()
                    {
                        Date = s.StudyRoomSession.Created,
                        Price = s.TotalPrice,
                        Duration = s.Duration,
                        TutorName = s.StudyRoomSession.StudyRoom.Tutor.User.Name,
                        TutorId = s.StudyRoomSession.StudyRoom.Tutor.Id,
                        TutorImage = _urlBuilder.BuildUserImageEndpoint(s.StudyRoomSession.StudyRoom.Tutor.Id, 
                            s.StudyRoomSession.StudyRoom.Tutor.User.ImageName, s.StudyRoomSession.StudyRoom.Tutor.User.Name, null)
                    }).ToFuture<UserPurchaseDto>();





                var buyPointsFuture = _session.Query<BuyPointsTransaction>()
                    .Fetch(s => s.User)
                    .Where(w => w.User.Id == query.Id)
                    .Select(s => new PurchasedBuyPointsDto()
                    {
                        Id = s.Id,
                        Price = s.Price,
                        Country = s.User.Country,
                        Date = s.Created
                    }).ToFuture<UserPurchaseDto>();

                var documentResult = await documentFuture.GetEnumerableAsync(token);
                var sessionResult = await sessionFuture.GetEnumerableAsync(token);
                var buyPointsResult = await buyPointsFuture.GetEnumerableAsync(token);
                var newSession = newSessionFuture.GetEnumerable();
                return documentResult.Union(sessionResult).Union(newSession).Union(buyPointsResult).OrderByDescending(o => o.Date);
            }
        }
    }
}
