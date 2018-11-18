using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    internal class UserMap : SpitballClassMap<User>
    {
        public UserMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='{nameof(User)}'");
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
            Map(e => e.Fictive).Update();
            Map(e => e.FraudScore);

            Map(e => e.PasswordHash).Nullable();
            Map(e => e.LockoutEnd).Nullable();
            Map(e => e.AccessFailedCount);
            Map(e => e.LockoutEnabled);
            Map(e => e.OldUser).Nullable();

            References(x => x.University).Column("UniversityId2").ForeignKey("User_University2").Nullable();
            Map(x => x.Balance).CustomSqlType("smallmoney");

            HasMany(x => x.Transactions)
                .Inverse()
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.Answers)
                .Inverse()
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.Questions)
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



            SchemaAction.None();
            /*
             * CREATE UNIQUE NONCLUSTERED INDEX idx_phoneNumber_notnull
               ON sb.[User](PhoneNumberHash)
               WHERE PhoneNumberHash IS NOT NULL;
             */
        }
    }
}
