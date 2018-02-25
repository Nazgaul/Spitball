using Cloudents.Core.DTOs;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data
{
    [UsedImplicitly]
    public class CourseSyncAzureSearchRepository : SyncAzureSearchRepository<CourseSearchWriteDto>
    {
        public CourseSyncAzureSearchRepository(DapperRepository repository) : base(repository)
        {
        }

        protected override string WriteSql => @"IF (@version > CHANGE_TRACKING_MIN_VALID_VERSION(  
                                   OBJECT_ID('zbox.box')))
select b.boxId as id,b.boxName as name,b.courseCode as code,b.isDeleted, b.university as UniversityId,
ct.sys_change_version as version
FROM  
    zbox.box b
RIGHT OUTER JOIN  
    CHANGETABLE(CHANGES zbox.box, @version) AS CT  
ON  
    b.boxId = CT.boxId  
join zbox.university u
on b.university = u.id and Latitude is not null	and Longitude is not null
else
select boxId as id,b.boxName as name,b.courseCode as code,b.isDeleted, b.university as UniversityId,
CHANGE_TRACKING_MIN_VALID_VERSION(  
                                   OBJECT_ID('zbox.box')) as version
FROM  
    zbox.box b
    join zbox.university u
on b.university = u.id and Latitude is not null	and Longitude is not null";

        protected override string DeleteSql =>
            @"select boxId as id, ct.sys_change_version as version from  CHANGETABLE(CHANGES zbox.box, @version) AS CT
where sys_change_operation = 'D';";
    }
}