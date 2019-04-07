﻿using System.Globalization;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    internal class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='User'");
            Map(e => e.Email)/*.Not.Nullable()*/.Unique();
            Map(e => e.Name).ReadOnly().Not.Nullable();
            Map(e => e.EmailConfirmed);
            
            Map(e => e.NormalizedName);
            Map(e => e.NormalizedEmail);
            Map(e => e.SecurityStamp);
            Map(e => e.Image).Length(5000).Nullable();
            Map(e => e.AuthenticatorKey);
           // Map(e => e.Culture);

           

            Map(e => e.Country).Nullable().Length(2);

            Map(e => e.Created).Insert().Not.Update();
            Map(e => e.Fictive).ReadOnly();

          
            Map(e => e.OldUser).Nullable();

            References(x => x.University).Column("UniversityId2").ForeignKey("User_University2").Nullable();
            Map(x => x.Language).Column("Language").CustomType<CultureInfo>().Nullable();

            HasMany(x => x.Questions).Access.CamelCaseField(Prefix.Underscore)
                .Inverse()
                .Cascade.AllDeleteOrphan();

            

            Map(x => x.Score).ReadOnly();
            //Table("User]"); //if not there is sql error
            
            SchemaAction.None();
            DiscriminateSubClassesOnColumn("Fictive");
            /*
             * CREATE UNIQUE NONCLUSTERED INDEX idx_phoneNumber_notnull
               ON sb.[User](PhoneNumberHash)
               WHERE PhoneNumberHash IS NOT NULL;
             */
        }
    }


    public class FictiveUserMap : SubclassMap<SystemUser>
    {
        public FictiveUserMap()
        {

            DiscriminatorValue(true);
        }
    }



    //public class UserRoleMap : ClassMap<UserRole>

    //{
    //    public UserRoleMap()
    //    {
            
    //        Id(x => x.Id).GeneratedBy.GuidComb();
    //        References(x => x.User).Not.Nullable().Column("UserId");
    //        Table("UserType");


    //    }
    //}
}
