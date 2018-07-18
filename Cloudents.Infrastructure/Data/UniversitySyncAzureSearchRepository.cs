using Cloudents.Core.DTOs;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data
{
    [UsedImplicitly]
    public class UniversitySyncAzureSearchRepository : SyncAzureSearchRepository<UniversitySearchWriteDto>
    {
        public UniversitySyncAzureSearchRepository(DapperRepository repository) : base(repository)
        {
        }

        protected override string WriteSql => @"IF (@version > CHANGE_TRACKING_MIN_VALID_VERSION(  
                                   OBJECT_ID('zbox.university')))
select u.id,universityName as name,largeImage as image,isDeleted,extra,
u.Longitude, Latitude, 
ct.sys_change_version as version
FROM  
    zbox.university u
RIGHT OUTER JOIN  
    CHANGETABLE(CHANGES zbox.university, @version) AS CT  
ON  
    u.id = CT.id  
	"+ WherePart + OrderByQueryWithPaging + @"
else
select u.id,universityName as name,largeImage as image,isDeleted,extra,
u.Longitude, Latitude, 
CHANGE_TRACKING_MIN_VALID_VERSION(  
                                   OBJECT_ID('zbox.university')) as version
FROM  
    zbox.university u" + WherePart + OrderByQueryWithPaging;

        protected override string DeleteSql => @"select id, ct.sys_change_version as version from  CHANGETABLE(CHANGES zbox.university, @version) AS CT
where sys_change_operation = 'D'" + OrderByQueryWithPaging;


        private const string OrderByQueryWithPaging = " order by id " + CourseSyncAzureSearchRepository.PagingQuery;

        private const string WherePart = @" where Latitude is not null 
	and Longitude is not null ";
    }
}
