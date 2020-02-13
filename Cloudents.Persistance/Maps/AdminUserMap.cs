using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class AdminUserMap : ClassMap<AdminUser>
    {
        public AdminUserMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Email).Not.Nullable().Unique();
            Map(x => x.Country).Nullable();
            References(r => r.Language).Column("LanguageId").ForeignKey("FK_user_language").Not.Nullable();
            //HasMany(x => x.Roles).AsSet();

            ReadOnly();

        }
    }


    //public class AdminUserRolesMap : ClassMap<AdminUserRoles>
    //{
    //    public AdminUserRolesMap()
    //    {
    //        Id(x => x.Id).GeneratedBy.GuidComb();
    //        Map(x => x.Role).Not.Nullable();

    //        ReadOnly();
    //    }
    //}
}