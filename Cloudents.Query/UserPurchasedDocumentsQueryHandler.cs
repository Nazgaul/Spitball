using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    public class UserPurchasedDocumentsQueryHandler : IQueryHandler<UserPurchaseDocumentByIdQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly DapperRepository _dapper;



        public UserPurchasedDocumentsQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }
        public async Task<IEnumerable<DocumentFeedDto>> GetAsync(UserPurchaseDocumentByIdQuery query, CancellationToken token)
        {


            return await _dapper.WithConnectionAsync(async connection =>
           {
               return await connection.QueryAsync<DocumentFeedDto, UserDto, VoteDto, DocumentFeedDto>(@"select
d.Id as id,
u.Name as University,
d.CourseName as 'Course',
d.MetaContent as snippet,
d.Name as Title,
d.Professor,
d.Type as 'Type',
d.Views,
d.Downloads,
'Cloudents' as Source,
d.UpdateTime as 'DateTime',
d.Price,
u2.id as Id,
u2.Name as Name,
u2.Score as Score,
u2.image as Image,
d.VoteCount as Votes
 from sb.[Transaction] t
Join sb.Document d on t.DocumentId = d.Id and d.state = 'ok' and t.action='PurchaseDocument'
join sb.University u on u.Id = d.UniversityId
join sb.[User] u2 on u2.Id = d.UserId
where t.User_id = @userId", (dto, userDto, arg3) =>
               {
                   dto.User = userDto;
                   dto.Vote = arg3;
                   return dto;
               }, new { userId = query.Id }, splitOn: "Id,Votes");
           }, token);
        }

    }
}