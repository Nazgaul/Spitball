using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public class DomainTimeStampMap : ComponentMap<DomainTimeStamp>
    {
        public DomainTimeStampMap()
        {
            Map(m => m.CreationTime).Insert().Not.Update();
            Map(m => m.UpdateTime).Insert();
        }
    }
}