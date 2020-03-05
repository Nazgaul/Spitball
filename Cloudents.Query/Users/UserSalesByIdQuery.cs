using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using PaymentStatus = Cloudents.Core.DTOs.PaymentStatus;
using System;

namespace Cloudents.Query.Users
{
    public class UserSalesByIdQuery : IQuery<IEnumerable<SaleDto>>
    {
        public UserSalesByIdQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }

        internal sealed class UserSalesByIdQueryHandler : IQueryHandler<UserSalesByIdQuery, IEnumerable<SaleDto>>
        {
            private readonly IStatelessSession _session;
            //private readonly IDapperRepository _dapper;

            public UserSalesByIdQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<SaleDto>> GetAsync(UserSalesByIdQuery query, CancellationToken token)
            {

                var documentFuture = _session.Query<DocumentTransaction>()
                    .Fetch(f => f.User)
                    .Fetch(f => f.Document)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Type == TransactionType.Earned)
                    .Where(w=>w.Document.Status.State == ItemState.Ok)
                    .Select(s => new DocumentSaleDto()
                    {
                        Id = s.Document.Id,
                        Name = s.Document.Name,
                        Course = s.Document.Course.Id,
                        Type = s.Document.DocumentType != null ? 
                            (ContentType)s.Document.DocumentType :
                            ContentType.Document,
                        Date = s.Created,
                        Price = s.Price
                    }).ToFuture<SaleDto>();


                var questionFuture = _session.Query<QuestionTransaction>()
                    .Fetch(f => f.Answer)
                    .Fetch(f => f.Question)
                    .Where(w=> w.Question != null)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Type == TransactionType.Earned)
                    .Select(s => new QuestionSaleDto()
                    {
                        Id = s.Question.Id,
                        Course = s.Question.Course.Id,
                        Date = s.Created,
                        Price = s.Price,
                        Text = s.Question.Text,
                        AnswerText = s.Answer.Text
                    }).ToFuture<SaleDto>();


                var sessionFuture = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Users)
                    .Where(w => w.StudyRoom.Tutor.Id == query.Id && w.Ended != null)
                    .Select(s => new SessionSaleDto()
                    {
                        PaymentStatus = string.IsNullOrEmpty(s.Receipt) ? PaymentStatus.Pending : PaymentStatus.Paid,
                        Date = s.Created,
                        Price = s.Price ?? 0,
                        StudentName = s.StudyRoom.Users.Where(w => w.User.Id != query.Id).Select(si => si.User.Name).FirstOrDefault(),
                        Duration = s.AdminDuration != null? TimeSpan.FromMinutes(Convert.ToDouble(s.AdminDuration)) : s.Duration,
                        StudentImage = s.StudyRoom.Users.Where(w => w.User.Id != query.Id).Select(si => si.User.ImageName).FirstOrDefault(),
                        StudentId = s.StudyRoom.Users.Where(w => w.User.Id != query.Id).Select(si => si.User.Id).FirstOrDefault()
                    }).ToFuture<SaleDto>();


                var documentResult = await documentFuture.GetEnumerableAsync(token);
                var questionResult = await questionFuture.GetEnumerableAsync(token);
                var sessionResult = await sessionFuture.GetEnumerableAsync(token);

                return documentResult.Union(questionResult).Union(sessionResult).OrderByDescending(o => o.Date);               
            }
        }
    }
}
