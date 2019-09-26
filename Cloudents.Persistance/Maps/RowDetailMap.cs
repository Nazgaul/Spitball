using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public class RowDetailMap : ComponentMapping<RowDetail>
    {
        public RowDetailMap()
        {
            Property(m => m.CreatedUser, c => {
                c.Insert(true);
                c.Update(false);
            });
            //Map(m => m.CreatedUser).Insert().Not.Update();
            Property(m => m.CreationTime, c => {
                c.Insert(true);
                c.Update(false);
            });
            //Map(m => m.CreationTime).Insert().Not.Update();
            Property(m => m.UpdateTime, c => c.Insert(true));
            //Map(m => m.UpdateTime).Insert();
            Property(m => m.UpdatedUser, c => c.Insert(true));
            //Map(m => m.UpdatedUser).Insert();
        }
    }
}