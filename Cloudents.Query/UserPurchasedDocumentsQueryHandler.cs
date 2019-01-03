using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Infrastructure.Data;
using Cloudents.Query.Query;
using Dapper;

namespace Cloudents.Query
{
    public class UserPurchasedDocumentsQueryHandler : IQueryHandler<UserPurchaseDocumentByIdQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly DapperRepository _dapper;
        private readonly IMapper _mapper;



        public UserPurchasedDocumentsQueryHandler(DapperRepository dapper, IMapper mapper, SqlConnection connection)
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
d.Price
 from sb.[Transaction] t
Join sb.Document d on t.DocumentId = d.Id and d.state = 'ok'
join sb.University u on u.Id = d.UniversityId
join sb.[User] u2 on u2.Id = d.UserId
where t.User_id = @userId", new { userId = query.Id });
            }, token);
        
            return _mapper.Map<IEnumerable<DocumentFeedDto>>(result);

        }




       

    }
}