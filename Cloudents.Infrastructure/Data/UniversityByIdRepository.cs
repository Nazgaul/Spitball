using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
   public  class UniversityByIdRepository : IReadRepositoryAsync<UniversityDto, long>
    {
        private readonly DapperRepository _repository;

        public UniversityByIdRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<UniversityDto> GetAsync(long query, CancellationToken token)
        {
            return _repository.WithConnectionAsync( c => c.QueryFirstAsync<UniversityDto>(new CommandDefinition(
                "SELECT id,UniversityName AS name, LargeImage AS image FROM zbox.university where id = @universityId", new {universityId = query},
                cancellationToken: token)), token);
        }
    }
}
