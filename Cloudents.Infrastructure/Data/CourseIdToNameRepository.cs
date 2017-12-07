using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class CourseIdToNameRepository : IReadRepositoryAsync<IEnumerable<CourseNameDto>, IEnumerable<long>>
    {
        private readonly DapperRepository _repository;

        public CourseIdToNameRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<CourseNameDto>> GetAsync(IEnumerable<long> query, CancellationToken token)
        {
            return _repository.WithConnectionAsync(c => c.QueryAsync<CourseNameDto>(
                @"SELECT COALESCE(CourseCode,BoxName) as Name 
FROM zbox.box WHERE IsDeleted = 0 and BoxId IN @BoxIds;",
                new { BoxIds = query }), token);
        }
    }
}