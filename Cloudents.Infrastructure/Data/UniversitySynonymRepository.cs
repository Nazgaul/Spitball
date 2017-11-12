using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class UniversitySynonymRepository :  IReadRepositorySingle<UniversitySynonymDto, long>
    {
        private readonly DapperRepository _repository;

        public UniversitySynonymRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<UniversitySynonymDto> GetAsync(long universityId, CancellationToken token)
        {
            return _repository.WithConnectionAsync(c => c.QueryFirstAsync<UniversitySynonymDto>("select coalesce( url, UniversityName) as Name from zbox.university where id=@universityId", new { universityId }), token);
        }
    }
}
