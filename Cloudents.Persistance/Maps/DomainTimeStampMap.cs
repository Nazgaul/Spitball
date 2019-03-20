using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
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