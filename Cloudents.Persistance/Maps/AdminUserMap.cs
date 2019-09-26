using Cloudents.Core.Entities;
using NHibernate.Mapping.ByCode;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;
using System.Collections.Generic;

namespace Cloudents.Persistence.Maps
{
    public class AdminUserMap : ClassMapping<AdminUser>
    {
        public AdminUserMap()
        { 
            Id(x => x.Id, m => m.Generator(Generators.GuidComb)); //.GeneratedBy.GuidComb();
            Property(x => x.Email, m => { m.NotNullable(true); m.Unique(true); });
            //Map(x => x.Email).Not.Nullable().Unique();
            Property(x => x.Country, m => m.NotNullable(false));
            //Map(x => x.Country).Nullable();
            //Set(x => x.Roles, c =>
            //{
            //    c.Table("AdminUserRoles");
            //}, r =>
            //{
            //    r.OneToMany(o => { });
            //});
            //HasMany(x => x.Roles).AsSet();

            //ReadOnly();
            Mutable(false);
        }
    }


    //public class AdminUserRolesMap : ClassMapping<AdminUserRoles>
    //{
    //    public AdminUserRolesMap()
    //    {
    //        Id(x => x.Id, m => m.Generator(Generators.GuidComb));
    //        Property(x => x.Role, m => m.NotNullable(true));
    //        Mutable(false);
    //    }
    //}
}