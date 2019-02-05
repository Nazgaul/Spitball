﻿using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Query.Email
{
    public class GetAnswerAcceptedEmailQuery :IQuery<AnswerAcceptedEmailDto>
    {
        public GetAnswerAcceptedEmailQuery(Guid transactionId)
        {
            TransactionId = transactionId;
        }

        private Guid TransactionId { get; }


        internal sealed class GetAnswerAcceptedEmailQueryQueryHandler : IQueryHandler<GetAnswerAcceptedEmailQuery, AnswerAcceptedEmailDto>
        {
            private readonly IConfigurationKeys _provider;

            public GetAnswerAcceptedEmailQueryQueryHandler(IConfigurationKeys provider)
            {
                _provider = provider;
            }

            public async Task<AnswerAcceptedEmailDto> GetAsync(GetAnswerAcceptedEmailQuery query, CancellationToken token)
            {
                const string sql = @"Select 
u.Email as ToEmailAddress,
u.Language,
 u.id as userId,
 t.Price as tokens,
 q.Text as questionText,
 a.Text as answerText
  from  sb.[Transaction] t
join sb.[User] u on t.User_id = u.Id
join sb.Question q on q.id = t.QuestionId
join sb.Answer a on a.Id = t.AnswerId
where t.id = @id ";
                using (var connection = new SqlConnection(_provider.Db.Db))
                {
                    return await connection.QuerySingleOrDefaultAsync<AnswerAcceptedEmailDto>(sql,
                        new
                        {
                            id = query.TransactionId,
                        });
                }
            }
        }
    }
}