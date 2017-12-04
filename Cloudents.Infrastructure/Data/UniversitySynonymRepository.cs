using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class UniversitySynonymRepository : IReadRepositoryAsync<UniversitySynonymDto, long>
    {
        private readonly DapperRepository _repository;

        public UniversitySynonymRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<UniversitySynonymDto> GetAsync(long universityId, CancellationToken token)
        {
            return _repository.WithConnectionAsync(async c =>
            {
                var dbResult = await c.QueryFirstAsync(
                    new CommandDefinition("select  UniversityName,Extra from zbox.university where id=@universityId", new { universityId }, cancellationToken: token)).ConfigureAwait(false);
                var result = new List<string> {dbResult.UniversityName.ToString()};
                string extra = dbResult.Extra?.ToString();

                if (extra != null)
                {
                    result.AddRange(extra.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                }
                return new UniversitySynonymDto
                {
                    Name = result
                };
            }, token);
        }
    }
}
