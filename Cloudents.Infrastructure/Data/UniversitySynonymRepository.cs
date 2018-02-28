using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Dapper;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data
{
    [UsedImplicitly]
    public class UniversitySynonymRepository : IReadRepositoryAsync<UniversitySynonymDto, long>
    {
        private readonly DapperRepository _repository;

        public UniversitySynonymRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<UniversitySynonymDto> GetAsync(long query, CancellationToken token)
        {
            return _repository.WithConnectionAsync(async c =>
            {
                var dbResult = await c.QueryFirstOrDefaultAsync<University>(
                    new CommandDefinition($"select UniversityName as {nameof(University.Name)},{nameof(University.ExtraSearch)} from zbox.university where {nameof(University.Id)}=@universityId", new { universityId = query }, cancellationToken: token)).ConfigureAwait(false);
                if (dbResult == null)
                {
                    return null;
                }
                var result = new List<string> { dbResult.Name };

                if (dbResult.ExtraSearch != null)
                {
                    result.AddRange(dbResult.ExtraSearch.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                }
                return new UniversitySynonymDto
                {
                    Name = result
                };
            }, token);
        }
    }
}
