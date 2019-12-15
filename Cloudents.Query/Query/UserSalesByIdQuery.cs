﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class UserSalesByIdQuery : IQuery<IEnumerable<SaleDto>>
    {
        public UserSalesByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }

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
                    .Where(w => w.Type == Core.Enum.TransactionType.Earned)
                    .Select(s => new DocumentSaleDto()
                    {
                        Id = s.Document.Id,
                        Name = s.Document.Name,
                        Course = s.Document.Course.Id,
                        Type = s.Document.DocumentType.ToString(),
                        Date = s.Created,
                        Price = s.Price
                    }).ToFuture<SaleDto>();


                var questionFuture = _session.Query<QuestionTransaction>()
                    .Fetch(f => f.Answer)
                    .Fetch(f => f.Question)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Type == Core.Enum.TransactionType.Earned)
                    .Select(s => new QuestionSaleDto()
                    {
                        Course = s.Question.Course.Id,
                        Date = s.Created,
                        Price = s.Price,
                        Text = s.Question.Text,
                        AnswerText = s.Answer.Text
                    }).ToFuture<SaleDto>();


                var sessionFuture = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Users)
                    .Where(w => w.StudyRoom.Tutor.Id == query.Id)
                    .Select(s => new SessionSaleDto() {
                        Status = string.IsNullOrEmpty(s.Receipt)? "Pending" : "Paid",
                        Date = s.Created,
                        Price = s.Price,
                        StudentName = s.StudyRoom.Users.Where(w => w.User.Id != query.Id).Select(si => si.User.Name).FirstOrDefault(),
                        Duration = s.Duration
                    }).ToFuture<SaleDto>();
          

                IEnumerable<SaleDto> documentResult = await documentFuture.GetEnumerableAsync(token);
                IEnumerable<SaleDto> questionResult = await questionFuture.GetEnumerableAsync(token);
                IEnumerable<SaleDto> sessionResult = await sessionFuture.GetEnumerableAsync(token);
               
                return documentResult.Union(questionResult).Union(sessionResult).OrderByDescending(o => o.Date);
                
            }
        }
    }
}
