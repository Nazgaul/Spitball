using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class UniversitySynonymRepository :  IReadRepositorySingle<UniversitySynonymDto, long>
    {
        private readonly DapperRepository m_Repository;

        public UniversitySynonymRepository(DapperRepository repository)
        {
            m_Repository = repository;
        }

        public Task<UniversitySynonymDto> GetAsync(long universityId, CancellationToken token)
        {
            return m_Repository.WithConnectionAsync(c => c.QueryFirstAsync<UniversitySynonymDto>("select coalesce( url, UniversityName) as Name from zbox.university where id=@universityId", new { universityId }), token);
        }
    }
}
