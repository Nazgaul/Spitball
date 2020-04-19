﻿using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Enum;
using Cloudents.Query.Sync;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Query.SearchSync
{
    public class DocumentSyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<DocumentSearchDto>,
    IQueryHandler<SyncAzureQuery, (IEnumerable<DocumentSearchDto> update, IEnumerable<string> delete, long version)>
    {


        public DocumentSyncAzureSearchQueryHandler(QuerySession session) : base(session)
        {
        }

        protected override string VersionSql
        {
            get
            {
                const string res = @"select d.Id as ItemId,
	                            d.Name as Name,
	                            (select cardDisplay from sb.course2 where id = d.CourseId2) as Course,
	                            d.DocumentType as Type,
	                            d.State as State,
	                            d.UpdateTime as DateTime, 
	                            u.Country as Country,
	                            c.* 
                            From sb.[Document] d  
                            right outer join CHANGETABLE (CHANGES sb.[Document], :Version) AS c ON d.Id = c.id 
                            Order by d.Id 
                            OFFSET :PageSize * :PageNumber 
                            ROWS FETCH NEXT :PageSize ROWS ONLY";
                return res;
            }
        }


        protected override string FirstQuery
        {
            get
            {
                const string res = @"
with cte as (

select d.Id as ItemId,	            
                d.Name as Name,	               
				(select cardDisplay from sb.course2 where id = d.CourseId2) as Course,	               
				d.DocumentType as Type,						                 
				d.State as State,				
				d.UpdateTime as DateTime, 	                   
				u.Country as Country,							
				u.Name as UniversityName	     		
				From sb.[Document] d  
				Order by d.Id                     		
				OFFSET  :PageSize * :PageNumber                  			
				ROWS FETCH NEXT :PageSize ROWS ONLY 
) 
select * from 
cte
CROSS APPLY CHANGETABLE (VERSION sb.[Document], (Id), (cte.ItemId)) AS c;";
                return res;
            }
        }

        protected override int PageSize => 200;


        protected override ILookup<bool, AzureSyncBaseDto<DocumentSearchDto>> SeparateUpdateFromDelete(IEnumerable<AzureSyncBaseDto<DocumentSearchDto>> result)
        {
            return result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D" || p.Data.State != ItemState.Ok);
        }
    }
}