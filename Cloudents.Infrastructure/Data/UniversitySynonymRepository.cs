using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class UniversitySynonymRepository : DapperRepository, IReadRepositorySingle<UniversitySynonymDto, long>
    {
        public UniversitySynonymRepository(string connectionString) : base(connectionString)
        {
        }

        public Task<UniversitySynonymDto> GetAsync(long universityId, CancellationToken token)
        {
            return WithConnection(c => c.QueryFirstAsync<UniversitySynonymDto>("select coalesce( url, UniversityName) from zbox.university where id=@universityId", new { universityId }), token);
        }

    }
}
