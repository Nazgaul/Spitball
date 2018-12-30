using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    [UsedImplicitly]
    public class DocumentByIdQueryHandler : IQueryHandler<DocumentById, DocumentDetailDto>
    {
        private readonly IStatelessSession _session;

        public DocumentByIdQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public async Task<DocumentDetailDto> GetAsync(DocumentById query, CancellationToken token)
        {
            var futureValue = _session.Query<Document>()

               .Fetch(f => f.University)
               .Fetch(f => f.User)
               .Where(w => w.Id == query.Id && w.State == ItemState.Ok)
               .Select(s => new DocumentDetailDto
               {
                   Id = s.Id,
                   Name = s.Name,
                   Date = s.TimeStamp.UpdateTime,
                   University = s.University.Name,
                   TypeStr = s.Type,
                   Pages = s.PageCount.GetValueOrDefault(),
                   Professor = s.Professor,
                   Views = s.Views,
                   Downloads = s.Downloads,
                   Price = s.Price,
                    // IsPurchased = s.Transactions.Count > 0 ? true : false,
                    User = new UserDto
                   {
                       Id = s.User.Id,
                       Name = s.User.Name,
                       Image = s.User.Image,
                       Score = s.User.Score
                   },
                   Course = s.Course.Name
               }).ToFutureValue();//.SingleOrDefaultAsync(token);

            IFutureValue<Transaction> purchaseFuture = null;
            if (query.UserId.HasValue)
            {
                purchaseFuture = _session.Query<Transaction>()
                    .Where(w => w.User.Id == query.UserId.Value && w.Document.Id == query.Id)
                    .ToFutureValue();



            }

            var result = await futureValue.GetValueAsync(token);
            result.IsPurchased = true;
            if (result.Price.GetValueOrDefault() > 0)
            {
                if (purchaseFuture == null)
                {
                    result.IsPurchased = false;

                }
                else
                {
                    var transactionResult = await purchaseFuture.GetValueAsync(token);
                    if (transactionResult == null)
                    {
                        result.IsPurchased = false;
                    }
                    else
                    {
                        result.IsPurchased = true;
                    }
                }

            }

            return result;

            //string sql = $@"select D.Id,
            //                         D.Name,
            //                         D.UpdateTime as 'Date',
            //                         Un.Name as University,
            //                         D.[Type] as TypeStr,
            //                         D.[PageCount] as Pages,
            //                         D.Professor as Professor,
            //                         D.Views,
            //                         D.Downloads,
            //                         D.Price as Price,
            //                         case D.Price when isnull(D.Price,0) then 1 else
            //                          case when isnull(count(T.Id), 0) = 0 then 0 
            //                          else 1 end 
            //                         end as IsPurchesed,
            //                         U.Id as UserId,
            //                         U.Name as  UserName,
            //                         U.Score as UserScore,
            //                         D.CourseName
            //                        from sb.Document D
            //                        join sb.University Un
            //                         on D.UniversityId = Un.Id
            //                        join sb.[user] U
            //                         on D.UserId = U.Id
            //                        left join sb.[Transaction] T
            //                         on D.Id = T.DocumentId and T.[User_id] = {query.UserId}
            //                        where D.[State] = 'Ok' and D.Id = {query.Id}
            //                        group by  D.Id,
            //                         D.Name,
            //                         D.UpdateTime,
            //                         Un.Name,
            //                         D.[Type],
            //                         D.[PageCount],
            //                         D.Professor,
            //                         D.Views,
            //                         D.Downloads,
            //                         D.Price,
            //                         U.Id,
            //                         U.Name,
            //                         U.Score,
            //                         D.CourseName,
            //                         D.Price";
            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var t = connection.Query<FlatDocumentDetailDto>(sql).FirstOrDefault();
            //    return null;
            //}

        }
    }
    //public class FlatDocumentDetailDto
    //{

    //    [DtoToEntityConnection(nameof(Document.Id))]
    //    public long Id { get; set; }

    //    [DtoToEntityConnection(nameof(Document.Name))]
    //    public string Name { get; set; }

    //    [DtoToEntityConnection(nameof(Document.TimeStamp))]
    //    public DateTime Date { get; set; }

    //    [DtoToEntityConnection(nameof(Document.University))]
    //    public string University { get; set; }

    //    [DtoToEntityConnection(nameof(Document.Course))]
    //    public string Course { get; set; }

    //    [DtoToEntityConnection(nameof(Document.Professor))]
    //    public string Professor { get; set; }



    //    [DtoToEntityConnection(nameof(Document.User.Id))]
    //    public long UserId { get; set; }
    //    [DtoToEntityConnection(nameof(Document.User.Name))]
    //    public string UserName { get; set; }


    //    [DtoToEntityConnection(nameof(Document.User.Score))]
    //    public int UserScore { get; set; }

    //    [DtoToEntityConnection(nameof(Document.MetaContent))]
    //    public string Extension => Path.GetExtension(Name)?.TrimStart('.');

    //    //[DataMember]
    //    [DtoToEntityConnection(nameof(Document.Type))]
    //    public DocumentType? TypeStr { get; set; }

    //    public string Type => TypeStr?.ToString("G");

    //    [DtoToEntityConnection(nameof(Document.PageCount))]
    //    public int? Pages { get; set; }

    //    [DtoToEntityConnection(nameof(Document.Views))]
    //    public int Views { get; set; }

    //    [DtoToEntityConnection(nameof(Document.Downloads))]
    //    public int Downloads { get; set; }

    //    [DtoToEntityConnection(nameof(Document.Price))]
    //    public decimal? Price { get; set; }

    //    [DtoToEntityConnection(nameof(Document.Transactions))]
    //    public bool IsPurchased { get; set; }

    //}
}