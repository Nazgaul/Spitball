using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Query.Users
{
    public class UserPurchasesByIdQuery : IQuery<IEnumerable<UserPurchasDto>>
    {
        public UserPurchasesByIdQuery(long id)
        {
            Id = id;
        }
        public long Id { get; set; }

        internal sealed class UserPurchasesByIdQueryHandler : IQueryHandler<UserPurchasesByIdQuery, IEnumerable<UserPurchasDto>>
        {
            private readonly IStatelessSession _session;
            private readonly IUrlBuilder _urlBuilder;

            public UserPurchasesByIdQueryHandler(IStatelessSession session, IUrlBuilder urlBuilder)
            {
                _session = session;
                _urlBuilder = urlBuilder;
            }

            public async Task<IEnumerable<UserPurchasDto>> GetAsync(UserPurchasesByIdQuery query, CancellationToken token)
            {
                var documentFuture = _session.Query<DocumentTransaction>()
                    .Fetch(f => f.User)
                    .Fetch(f => f.Document)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Document.Status.State == Core.Enum.ItemState.Ok)
                    .Where(w => w.Type == Core.Enum.TransactionType.Spent)
                    .Select(s => new PurchasedDocumentDto()
                    {
                        Id = s.Document.Id,
                        Name = s.Document.Name,
                        Course = s.Document.Course.Id,
                        Type = s.Document.DocumentType != null ?
                            (ContentType)s.Document.DocumentType :
                            ContentType.Document,
                        Date = s.Created,
                        Price = s.Price

                    }).ToFuture<UserPurchasDto>();

                var sessionFuture = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Users)
                    .Where(w => w.StudyRoom.Users.Select(s => s.User.Id).Any(a => a == query.Id) && query.Id != w.StudyRoom.Tutor.Id)
                    .Where(w => w.Ended != null)
                    .Select(s => new PurchasedSessionDto()
                    {
                        Date = s.Created,
                        Price = s.Price,
                        Duration = s.Duration,
                        TutorName = s.StudyRoom.Tutor.User.Name,
                        TutorId = s.StudyRoom.Tutor.Id,
                        TutorImage = _urlBuilder.BuildUserImageEndpoint(s.StudyRoom.Tutor.Id, s.StudyRoom.Tutor.User.ImageName, s.StudyRoom.Tutor.User.Name, null)
                    }).ToFuture<UserPurchasDto>();

                var buyPointsFuture = _session.Query<BuyPointsTransaction>()
                    .Fetch(s => s.User)
                    .Where(w => w.User.Id == query.Id)
                    .Select(s => new PurchasedBuyPointsDto()
                    { 
                        Price = s.Price,
                        Date = s.Created
                    }).ToFuture<UserPurchasDto>();

                IEnumerable<UserPurchasDto> documentResult = await documentFuture.GetEnumerableAsync(token);
                IEnumerable<UserPurchasDto> sessionResult = await sessionFuture.GetEnumerableAsync(token);
                IEnumerable<UserPurchasDto> buyPointsResult = await buyPointsFuture.GetEnumerableAsync(token);

                return documentResult.Union(sessionResult).Union(buyPointsResult).OrderByDescending(o => o.Date);
            }
        }
    }
}
