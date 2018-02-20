using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class UniversitySyncAzureSearchRepository : IReadRepositoryAsync<(List<UniversityWriteDto> update, IEnumerable<long> delete, long version), long>
    {
        private readonly DapperRepository _repository;

        public UniversitySyncAzureSearchRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<(List<UniversityWriteDto> update, IEnumerable<long> delete, long version)> GetAsync(long query, CancellationToken token)
        {
            const string writeSql = @"IF (@version > CHANGE_TRACKING_MIN_VALID_VERSION(  
                                   OBJECT_ID('zbox.university')))
select u.id,universityName as name,largeImage as image,isDeleted,
u.Longitude, Latitude, 
ct.sys_change_version as version
FROM  
    zbox.university u
RIGHT OUTER JOIN  
    CHANGETABLE(CHANGES zbox.university, 51461) AS CT  
ON  
    u.id = CT.id  
	where Latitude is not null 
	and Longitude is not null
else
select u.id,universityName as name,largeImage as image,isDeleted,
u.Longitude, Latitude, 
CHANGE_TRACKING_MIN_VALID_VERSION(  
                                   OBJECT_ID('zbox.university')) as version
FROM  
    zbox.university u;";
            const string deleteSql =
                @"select id, ct.sys_change_version as version from  CHANGETABLE(CHANGES zbox.university, 0) AS CT
where sys_change_operation = 'D';";
            return _repository.WithConnectionAsync(async c =>
            {
                using (var grid = await c.QueryMultipleAsync(writeSql + deleteSql, new { version = query }))
                {
                    var write = (await grid.ReadAsync<UniversityWriteDto>().ConfigureAwait(false)).ToList();
                    var delete = (await grid.ReadAsync()).ToList();

                    var max = write.Max(m => m.Version);
                    var maxDelete = delete.Max(m => (long)m.version);

                    return (write, delete.Select(s => (long)s.id), Math.Max(max, maxDelete));
                }
            }, token);
        }
    }
}
