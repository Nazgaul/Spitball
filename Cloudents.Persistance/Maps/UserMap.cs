﻿using Cloudents.Core.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistance.Maps
{
    internal class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='User'");
            Map(e => e.Email)/*.Not.Nullable()*/.Unique();
            Map(e => e.PrivateKey);
            Map(e => e.PhoneNumber).Column("PhoneNumberHash");
            Map(e => e.Name).Not.Nullable().Unique();
            Map(e => e.EmailConfirmed);
            Map(e => e.PhoneNumberConfirmed);
            Map(e => e.NormalizedName);
            Map(e => e.NormalizedEmail);
            Map(e => e.SecurityStamp);
            Map(e => e.Image).Nullable();
            Map(e => e.TwoFactorEnabled);
            Map(e => e.AuthenticatorKey);
            Map(e => e.Culture);
            Map(e => e.Country).Nullable().Length(2);

            Map(e => e.Created).Insert().Not.Update();
            Map(e => e.Fictive).ReadOnly();
            Map(e => e.FraudScore);

            Map(e => e.PasswordHash).Nullable();
            Map(e => e.LockoutEnd).Nullable();
            Map(e => e.AccessFailedCount);
            Map(e => e.LockoutEnabled);
            Map(e => e.OldUser).Nullable();

            Map(e => e.Score);
            References(x => x.University).Column("UniversityId2").ForeignKey("User_University2").Nullable();

            Map(x => x.Balance).CustomSqlType("smallmoney");

            HasMany(x => x.Transactions)
                .Inverse()
                .Cascade.AllDeleteOrphan();

           

            HasMany(x => x.Questions).Access.CamelCaseField(Prefix.Underscore)
                .Inverse()
                .Cascade.AllDeleteOrphan();
            
            HasMany(x => x.UserLogins)
                .Inverse()
                .Cascade.AllDeleteOrphan();

            //Map(x => x.Languages).CustomType<JsonType<ISet<CultureInfo>>>();


            HasManyToMany(x => x.Courses)
                .ParentKeyColumn("UserId")
                .ChildKeyColumn("CourseId")
                .ForeignKeyConstraintNames("User_Courses","Courses_User")
                .Table("UsersCourses").AsSet();


            HasManyToMany(x => x.Tags)
                .ParentKeyColumn("UserId")
                .ChildKeyColumn("TagId")
                .ForeignKeyConstraintNames("User_Tags", "Tags_User")
                .Table("UsersTags").AsSet();

            Table("[User]");

            SchemaAction.None();
            DiscriminateSubClassesOnColumn("Fictive");
            /*
             * CREATE UNIQUE NONCLUSTERED INDEX idx_phoneNumber_notnull
               ON sb.[User](PhoneNumberHash)
               WHERE PhoneNumberHash IS NOT NULL;
             */
        }
    }


    public class RegularUserMap : SubclassMap<RegularUser>
    {
        public RegularUserMap()
        {

            DiscriminatorValue(false);

            HasMany(x => x.Answers).Access.CamelCaseField(Prefix.Underscore).Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }

    public class FictiveUserMap : SubclassMap<SystemUser>
    {
        public FictiveUserMap()
        {

            DiscriminatorValue(true);
        }
    }
}
