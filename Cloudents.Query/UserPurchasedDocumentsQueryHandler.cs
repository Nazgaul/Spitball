using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Infrastructure.Data;
using Cloudents.Query.Query;
using Dapper;

namespace Cloudents.Query
{
    public class UserPurchasedDocumentsQueryHandler : IQueryHandler<UserPurchaseDocumentByIdQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly DapperRepository _dapper;
        private readonly IMapper _mapper;



        public UserPurchasedDocumentsQueryHandler(DapperRepository dapper, IMapper mapper)
        {
            _dapper = dapper;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DocumentFeedDto>> GetAsync(UserPurchaseDocumentByIdQuery query, CancellationToken token)
        {
            

              var result = await _dapper.WithConnectionAsync(async connection =>
            {
                return await connection.QueryAsync<UserPurchasedDocumentsQueryResult>(@"select d.Id as id,
u2.id as userId,
u2.Name as userName,
u2.Score as userScore,
d.UpdateTime as 'DateTime',
d.CourseName as 'Course',
d.Type as 'TypeStr',
d.Professor,
d.Name as Title,
d.Views,
d.Downloads,
u.Name as University,
d.MetaContent as Snippet,
d.VoteCount,
d.Price,
'Cloudents' as Source
 from sb.[Transaction] t
Join sb.Document d on t.DocumentId = d.Id and d.state = 'ok'
join sb.University u on u.Id = d.UniversityId
join sb.[User] u2 on u2.Id = d.UserId
where t.User_id = @userId", new { userId = query.Id });
            }, token);
        
            return _mapper.Map<IEnumerable<DocumentFeedDto>>(result);

        }

    }


    public class UserPurchasedDocumentsQueryResult
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int UserScore { get; set; }
        public DateTime? DateTime { get; set; }
        public string Course { get; set; }
        public DocumentType? TypeStr { get; set; }
        public string Professor { get; set; }
        public string Title { get; set; }
        public int? Views { get; set; }
        public decimal? Price { get; set; }
        public int? Downloads { get; set; }
        public string University { get; set; }
        public string Snippet { get; set; }
        public int VoteCount { get; set; }
        public string Source { get; set; }

    }
}