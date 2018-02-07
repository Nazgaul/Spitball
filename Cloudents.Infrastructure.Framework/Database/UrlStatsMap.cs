﻿using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Framework.Database
{
    public class UrlStatsMap : ClassMap<UrlStats>
    {
        public UrlStatsMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(e => e.Host);
            Map(e => e.DateTime);
            Map(e => e.UrlSource).Length(8000);
            Map(e => e.UrlTarget).Length(8000);
            Map(e => e.SourceLocation);
            Map(e => e.AggregateCount);
            Map(e => e.Ip);
        }
    }
}