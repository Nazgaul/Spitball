using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class UniversitySyncAzureSearchRepository : IReadRepositoryAsync<(List<UniversitySearchWriteDto> update, IEnumerable<UniversitySearchDeleteDto> delete, long version), long>
    {
        private readonly DapperRepository _repository;

        public UniversitySyncAzureSearchRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<(List<UniversitySearchWriteDto> update, IEnumerable<UniversitySearchDeleteDto> delete, long version)> GetAsync(long query, CancellationToken token)
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
    zbox.university u
    where Latitude is not null 
	and Longitude is not null;";
            const string deleteSql =
                @"select id, ct.sys_change_version as version from  CHANGETABLE(CHANGES zbox.university, @version) AS CT
where sys_change_operation = 'D';";
            return _repository.WithConnectionAsync(async c =>
            {
                using (var grid = await c.QueryMultipleAsync(writeSql + deleteSql, new { version = query }))
                {
                    var write = (await grid.ReadAsync<UniversitySearchWriteDto>().ConfigureAwait(false)).ToList();
                    var delete = (await grid.ReadAsync()).ToList();

                    long max = 0, maxDelete = 0;
                    if (write.Count > 0)
                    {
                        max = write.Max(m => m.Version);
                    }
                    if (delete.Count > 0)
                    {
                        maxDelete = delete.Max(m => (long)m.version);
                    }
                    return (write, delete.Select(s => new UniversitySearchDeleteDto { Id = (long)s.id }), new[] { max, maxDelete, query }.Max());
                }
            }, token);
        }
    }
}
