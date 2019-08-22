using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cloudents.Query
{
    [UsedImplicitly]
    public class UserPurchasedDocumentsQueryHandler : IQueryHandler<UserPurchaseDocumentByIdQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly IDapperRepository _dapper;



        public UserPurchasedDocumentsQueryHandler(IDapperRepository dapper)
        {
            _dapper = dapper;
        }
        public async Task<IEnumerable<DocumentFeedDto>> GetAsync(UserPurchaseDocumentByIdQuery query, CancellationToken token)
        {
            using (var connection = _dapper.OpenConnection())
            {
                return await connection.QueryAsync<DocumentFeedDto, DocumentUserDto, VoteDto, DocumentFeedDto>(@"select
d.Id as id,
d.University as University,
d.Course as 'Course',
d.Snippet as snippet,
d.Title as Title,
d.Views,
d.Downloads,
'Cloudents' as Source,
d.[DateTime] as 'DateTime',
d.Price,
d.User_id as Id,
d.User_Name as Name,
d.User_Score as Score,
d.User_image as Image,
d.User_IsTutor as IsTutor,
d.Vote_Votes as Votes
 from sb.[Transaction] t
Join [sb].[iv_DocumentSearch] d on t.DocumentId = d.Id and t.action='PurchaseDocument'
where t.User_id = @userId", (dto, userDto, arg3) =>
                {
                    dto.User = userDto;
                    dto.Vote = arg3;
                    return dto;
                }, new { userId = query.Id }, splitOn: "Id,Votes");
            }

            

        }

    }
}