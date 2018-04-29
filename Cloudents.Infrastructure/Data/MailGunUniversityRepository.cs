using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Dapper;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data
{
    [UsedImplicitly]
    public class MailGunUniversityRepository : IReadRepositoryAsync<IEnumerable<MailGunUniversityDto>>
    {
        private readonly DapperRepository _dapper;

        public MailGunUniversityRepository(DapperRepository.Factory dapper)
        {
            _dapper = dapper.Invoke(Core.Enum.Database.MailGun);
        }

        public Task<IEnumerable<MailGunUniversityDto>> GetAsync(CancellationToken token)
        {
            return _dapper.WithConnectionAsync(conn => conn.QueryAsync<MailGunUniversityDto>(new CommandDefinition(
                @"SELECT DISTINCT UniId as Id FROM dbo.students2
                WHERE ShouldSend = 1", cancellationToken: token)),token);
        }
    }
}
