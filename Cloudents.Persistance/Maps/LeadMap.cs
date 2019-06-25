﻿using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class LeadMap :ClassMap<Lead>
    {
        public LeadMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.Course).Not.Nullable();
            References(x => x.User).Nullable();
            References(x => x.Tutor).Nullable();
            Map(x => x.UtmSource);
            Map(x => x.Referer).Length(400);
            Map(x => x.Text).Length(1000);
            Map(x => x.CreationTime);
        }
        
    }
}