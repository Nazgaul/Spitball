using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Query
{
    public class MailGunQuery : IQuery<IEnumerable<MailGunDto>>
    { 

    public MailGunQuery(long id, int limitPerSession)
    {
        Id = id;
        LimitPerSession = limitPerSession;
    }
   
        public long Id { get; set; }
        public int LimitPerSession { get; set; }
    }
}
