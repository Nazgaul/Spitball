using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public class RowDetailMap : ComponentMap<RowDetail>
    {
        public RowDetailMap()
        {
            Map(m => m.CreatedUser).Insert().Not.Update();
            Map(m => m.CreationTime).Insert().Not.Update();
            Map(m => m.UpdateTime).Insert();
            Map(m => m.UpdatedUser).Insert();
        }
    }
}