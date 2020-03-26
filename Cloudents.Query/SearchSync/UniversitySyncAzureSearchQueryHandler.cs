//using Cloudents.Core.DTOs.SearchSync;
//using Cloudents.Core.Enum;
//using Cloudents.Query.Sync;
//using System.Collections.Generic;
//using System.Linq;

//namespace Cloudents.Query.SearchSync
//{
//    public class UniversitySyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<UniversitySearchDto>,
//        IQueryHandler<SyncAzureQuery, (IEnumerable<UniversitySearchDto> update, IEnumerable<string> delete, long version)>
//    {
//        public UniversitySyncAzureSearchQueryHandler(QuerySession session) : base(session)
//        {
//        }

//        protected override string VersionSql
//        {
//            get
//            {
//                const string res = @"select u.Id as UniversityId,
//	                            u.Name as Name,
//	                            u.Extra as Extra,
//	                            u.Country as Country,
//u.State as State,
//ImageUrl as Image,
//UsersCount,
//	                            c.* 
//                            From sb.[University] u  
//                            right outer join CHANGETABLE (CHANGES sb.[University], :Version) AS c ON u.Id = c.id   
//                            Order by u.Id 
//                            OFFSET :PageSize * :PageNumber ROWS 
//                            FETCH NEXT :PageSize ROWS ONLY";
//                return res;
//            }
//        }

//        protected override string FirstQuery
//        {
//            get
//            {
//                const string res = @"select u.Id as UniversityId,
//	                            u.Name as Name,
//	                            u.Extra as Extra,
//	                            u.Country as Country,
//u.State as State,
//ImageUrl as Image,
//UsersCount,
//	                            c.* 
//                            From sb.[University] u  

//                            CROSS APPLY CHANGETABLE (VERSION sb.[University], (Id), (u.Id)) AS c  
//                            Order by u.Id 
//                            OFFSET :PageSize * :PageNumber ROWS 
//                            FETCH NEXT :PageSize ROWS ONLY";
//                return res;
//            }
//        }

//        protected override int PageSize => 500;


//        protected override ILookup<bool, AzureSyncBaseDto<UniversitySearchDto>> SeparateUpdateFromDelete(IEnumerable<AzureSyncBaseDto<UniversitySearchDto>> result)
//        {
//            return result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D" || p.Data.State.GetValueOrDefault(ItemState.Ok) != ItemState.Ok);
//        }
//    }
//}