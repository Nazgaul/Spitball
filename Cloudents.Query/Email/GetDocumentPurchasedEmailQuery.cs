using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
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
            private readonly IConfigurationKeys _provider;

            public GetDocumentPurchasedEmailQueryHandler(IConfigurationKeys provider)
            {
                _provider = provider;
            }

            public async Task<DocumentPurchaseEmailDto> GetAsync(GetDocumentPurchasedEmailQuery query, CancellationToken token)
            {
                const string sql = @"Select 
 d.CourseName,
  d.Name as documentName ,
u.Email,
 u.id as userId,
 t.Price as tokens,
u.Language
  from  sb.[Transaction] t
join sb.[User] u on t.User_id = u.Id
join sb.Document d on t.DocumentId = d.id
where t.id = @id";
                using (var connection = new SqlConnection(_provider.Db.Db))
                {
                    return await connection.QueryFirstOrDefaultAsync<DocumentPurchaseEmailDto>(sql,
                        new
                        {
                            id = query.TransactionId,
                        });


                    //var result = results.First();
                    //var retVal = new DocumentPurchaseEmailDto()
                    //{
                    //    Language = result.Language,
                    //    CourseName = result.CourseName,
                       
                    //    DocumentName = result.DocumentName,
                    //    ToEmailAddress = result.Email,
                    //    UserId = result.UserId,
                    //    Tokens = result.Tokens
                        

                    //};
                    //foreach (var block in results)
                    //{
                    //    var emailBlock = new EmailBlockDto()
                    //    {
                    //        Title = block.Title,
                    //        Body = block.Body,
                    //        Subtitle = block.Subtitle,
                    //        Cta = block.Cta,
                    //        MinorTitle = block.MinorTitle
                    //    };
                    //    retVal.Blocks.Add(emailBlock);
                    //}
                    //return retVal;
                }
            }
        }

        private class DbClass
        {
            [DtoToEntityConnection(nameof(Document.Course.Id))]
            public string CourseName { get; set; }
            [DtoToEntityConnection(nameof(Document.Name))]
            public string DocumentName { get; set; }
            [DtoToEntityConnection(nameof(RegularUser.Email))]
            public string Email { get; set; }
            [DtoToEntityConnection(nameof(RegularUser.Id))]
            public long UserId { get; set; }

            [DtoToEntityConnection(nameof(Transaction.Price))]
            public decimal Tokens { get; set; }

        }



    }
}