﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Email
{
    public class GetDocumentPurchasedEmailQuery : IQuery<DocumentPurchaseEmailDto>
    {
        public GetDocumentPurchasedEmailQuery(Guid transactionId)
        {
            TransactionId = transactionId;
        }

        private Guid TransactionId { get; }



        internal sealed class GetDocumentPurchasedEmailQueryHandler : IQueryHandler<GetDocumentPurchasedEmailQuery, DocumentPurchaseEmailDto>
        {
            private readonly DapperRepository _dapper;

            public GetDocumentPurchasedEmailQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<DocumentPurchaseEmailDto> GetAsync(GetDocumentPurchasedEmailQuery query, CancellationToken token)
            {
                const string sql = @"Select 
 d.CourseName,
  d.Name as documentName ,
u.Email as ToEmailAddress,
 u.id as userId,
 t.Price as tokens,
u.Language
  from  sb.[Transaction] t
join sb.[User] u on t.User_id = u.Id
join sb.Document d on t.DocumentId = d.id
where t.id = @id";
                using (var connection = _dapper.OpenConnection())
                {
                    return await connection.QuerySingleAsync<DocumentPurchaseEmailDto>(sql,
                        new
                        {
                            id = query.TransactionId,
                        });

                }
            }
        }
    }
}