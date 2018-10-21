﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Dapper;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data
{
    [UsedImplicitly]
    public class MailGunDataRepository : IReadRepositoryAsync<IEnumerable<MailGunDto>,long>
         , IReadRepositoryAsync<IEnumerable<MailGunDto>, MailGunQuery>
    {
        private readonly DapperRepository _dapper;

        public MailGunDataRepository(DapperRepository.Factory dapper)
        {
            _dapper = dapper.Invoke(Core.Enum.Database.MailGun);
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


        public Task<IEnumerable<MailGunDto>> GetAsync(MailGunQuery query, CancellationToken token)
        {
            
            return _dapper.WithConnectionAsync(conn => conn.QueryAsync<MailGunDto>(new CommandDefinition(
                @"SELECT top (@top) s.id, FirstName, LastName, Email,mailBody as MailBody,
mailSubject as MailSubject, mailCategory as MailCategory
from students2 s 
where uniId = @UniId
and shouldSend = 1
and chapter is null
order by s.id;", new { UniId = query.Id, top = query.LimitPerSession }, cancellationToken: token)), token);
        }
    }
}