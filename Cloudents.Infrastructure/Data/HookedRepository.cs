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
    public class HookedRepository : IReadRepositoryAsync<IEnumerable<HookedDto>, IEnumerable<string>>
    {
        private readonly DapperRepository _repository;

        public HookedRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<HookedDto>> GetAsync(IEnumerable<string> query, CancellationToken token)
        {
            return _repository.WithConnectionAsync(c => c.QueryAsync<HookedDto> (
                "select Id from hooked where id in @Ids", new { Ids = query }), token);
        }
    }
}
