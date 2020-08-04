//using Cloudents.Core.DTOs.Email;
//using Dapper;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;
//using NHibernate;

//namespace Cloudents.Query.Email
//{
//    public class GetDocumentPurchasedEmailQuery : IQuery<DocumentPurchaseEmailDto>
//    {
//        public GetDocumentPurchasedEmailQuery(Guid transactionId)
//        {
//            TransactionId = transactionId;
//        }

//        private Guid TransactionId { get; }



//        internal sealed class GetDocumentPurchasedEmailQueryHandler : IQueryHandler<GetDocumentPurchasedEmailQuery, DocumentPurchaseEmailDto>
//        {
//            private readonly IStatelessSession _dapper;

//            public GetDocumentPurchasedEmailQueryHandler(IStatelessSession dapper)
//            {
//                _dapper = dapper;
//            }

//            public async Task<DocumentPurchaseEmailDto> GetAsync(GetDocumentPurchasedEmailQuery query, CancellationToken token)
//            {
//                _dapper.Query<DocumentTransaction>()
//                    .Where(w=>w.Id == query.TransactionId)
//                    .Select(s=> new DocumentPurchaseEmailDto()
//                    {
//                        UserId = s.User.Id,
//                        CourseName = s.Document.Course.Name,
//                        DocumentName = s.Document.Name,
//                        Language = s.User.Language,
//                        ToEmailAddress = s.D
//                    })
//                const string sql = @"Select 
// d.CourseName,
//  d.Name as documentName ,
//u.Email as ToEmailAddress,
// u.id as userId,
// t.Price as tokens,
//u.Language
//  from  sb.[Transaction] t
//join sb.[User] u on t.User_id = u.Id
//join sb.Document d on t.DocumentId = d.id
//where t.id = @id";
//                using var connection = _dapper.OpenConnection();
//                return await connection.QuerySingleAsync<DocumentPurchaseEmailDto>(sql,
//                    new
//                    {
//                        id = query.TransactionId,
//                    });
//            }
//        }
//    }
//}