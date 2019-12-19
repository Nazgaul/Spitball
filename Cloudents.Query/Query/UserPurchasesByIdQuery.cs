using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
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

            public UserPurchasesByIdQueryHandler(IStatelessSession session)
            {
                _session = session;
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
                    .Select(s => new PurchasedSessionDto()
                    {
                        Date = s.Created,
                        Price = s.Price,
                        Duration = s.Duration,
                        TutorName = s.StudyRoom.Tutor.User.Name,
                        TutorId = s.StudyRoom.Tutor.Id,
                        TutorImage = s.StudyRoom.Tutor.User.Image
                    }).ToFuture<UserPurchasDto>();


                IEnumerable<UserPurchasDto> documentResult = await documentFuture.GetEnumerableAsync(token);
                IEnumerable<UserPurchasDto> sessionResult = await sessionFuture.GetEnumerableAsync(token);

                return documentResult.Union(sessionResult).OrderByDescending(o => o.Date);
            }
        }
    }
}
