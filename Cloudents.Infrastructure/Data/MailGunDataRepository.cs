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
    public class MailGunDataRepository : IReadRepositoryAsync<IEnumerable<MailGunDto>,long>
    {
        private readonly DapperRepository _dapper;

        public MailGunDataRepository(DapperRepository.Factory dapper)
        {
            _dapper = dapper.Invoke(Database.MailGun);
        }

        public Task<IEnumerable<MailGunDto>> GetAsync(long query, CancellationToken token)
        {
            const int limitPerSession = 50;
            return _dapper.WithConnectionAsync(conn => conn.QueryAsync<MailGunDto>(new CommandDefinition(
                @"SELECT top (@top) s.id, FirstName, LastName, Email,mailBody as MailBody,
mailSubject as MailSubject, mailCategory as MailCategory
from students2 s 
where uniId = @UniId
and shouldSend = 1
and chapter is null
order by s.id;", new { UniId = query, top = limitPerSession }, cancellationToken: token)), token);
        }
    }
}