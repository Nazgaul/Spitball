using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Dapper;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Query
{
    public class GetDocumentPurchasedEmail : IQuery<DocumentPurchaseEmailDto>
    {
        public GetDocumentPurchasedEmail(Guid transactionId)
        {
            TransactionId = transactionId;
        }

        public Guid TransactionId { get; }

        internal sealed class GetDocumentPurchasedEmailQueryHandler : IQueryHandler<GetDocumentPurchasedEmail, DocumentPurchaseEmailDto>
        {
            private readonly IConfigurationKeys _provider;

            public GetDocumentPurchasedEmailQueryHandler(IConfigurationKeys provider)
            {
                _provider = provider;
            }

            public async Task<DocumentPurchaseEmailDto> GetAsync(GetDocumentPurchasedEmail query, CancellationToken token)
            {
                const string sql = @"Select e.language, e.SocialShare,e.Subject,
 ek.Title,
 ek.Subtitle,
 ek.Body,
 ek.Cta,
 d.CourseName,
  d.Name as documentName ,
u.Email
  from  sb.[Transaction] t
join sb.[User] u on t.User_id = u.Id
join sb.Email e on u.Language = e.Language and e.Event=@e
join sb.emailBlock ek on e.Id = ek.email_id
join sb.Document d on t.DocumentId = d.id
where t.id = @id
order by ek.orderBlock ";
                using (var connection = new SqlConnection(_provider.Db.Db))
                {
                    var results = await connection.QueryAsync< DbClass>(sql,
                        new
                        {
                            id = query.TransactionId,
                            e = (string)SystemEvent.DocumentPurchased
                        });


                    var result = results.First();
                    var retVal = new DocumentPurchaseEmailDto()
                    {
                        Language = result.Language,
                        CourseName = result.CourseName,
                        SocialShare = result.SocialShare,
                        Subject = result.Subject,
                        DocumentName = result.DocumentName,
                        ToEmailAddress = result.Email


                    };
                    foreach (var block in results)
                    {
                        var emailBlock = new EmailBlockDto()
                        {
                            Title = block.Title,
                            Body = block.Body,
                            Subtitle = block.Subtitle,
                            Cta = block.Cta
                        };
                        retVal.Blocks.Add(emailBlock);
                    }
                    return retVal;
                }
            }
        }

        private class DbClass
        {
            public string Language { get; set; }
            public bool SocialShare { get; set; }
            public string Subject { get; set; }
            public string Title { get; set; }
            public string Subtitle { get; set; }
            public string Body { get; set; }
            public string Cta { get; set; }
            public string CourseName { get; set; }
            public string DocumentName { get; set; }
            public string Email { get; set; }
        }



    }
}