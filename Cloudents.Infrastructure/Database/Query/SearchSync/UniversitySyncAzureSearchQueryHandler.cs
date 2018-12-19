using System.Collections.Generic;
using Cloudents.Application.DTOs.SearchSync;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query.Sync;

namespace Cloudents.Infrastructure.Database.Query.SearchSync
{
    public class UniversitySyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<UniversitySearchDto>,
        IQueryHandler<SyncAzureQuery, (IEnumerable<UniversitySearchDto> update, IEnumerable<string> delete, long version)>
    {
        public UniversitySyncAzureSearchQueryHandler(QuerySession session) : base(session)
        {
        }

        protected override string VersionSql
        {
            get
            {
                var res = @"select u.Id as UniversityId,
	                            u.Name as Name,
	                            u.Extra as Extra,
	                            u.Country as Country,
	                            c.* 
                            From sb.[University] u  
                            right outer join CHANGETABLE (CHANGES sb.[University], :Version) AS c ON u.Id = c.id   
                            Order by u.Id 
                            OFFSET :PageSize * :PageNumber ROWS 
                            FETCH NEXT :PageSize ROWS ONLY";
                return res;
            }
        }

        protected override string FirstQuery
        {
            get
            {
                var res = @"select u.Id as UniversityId,
	                            u.Name as Name,
	                            u.Extra as Extra,
	                            u.Country as Country,
	                            c.* 
                            From sb.[University] u  
                            CROSS APPLY CHANGETABLE (VERSION sb.[University], (Id), (u.Id)) AS c  
                            Order by u.Id 
                            OFFSET :PageSize * :PageNumber ROWS 
                            FETCH NEXT :PageSize ROWS ONLY";
                return res;
            }
        }

        protected override int PageSize => 500;
    }
}