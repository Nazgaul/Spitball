using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public class DomainTimeStampMap : ComponentMapping<DomainTimeStamp>
    {
        public DomainTimeStampMap()
        {
            Property(m => m.CreationTime, c => {
                c.Insert(true);
                c.Update(false);
            });
            //Map(m => m.CreationTime).Insert().Not.Update();
            Property(m => m.UpdateTime, c=> {
                c.Insert(true);
            });
            //Map(m => m.UpdateTime).Insert();
        }
    }
}