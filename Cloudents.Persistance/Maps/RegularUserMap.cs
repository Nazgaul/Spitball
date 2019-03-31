using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class RegularUserMap : SubclassMap<RegularUser>
    {
        public RegularUserMap()
        {

            DiscriminatorValue(false);
            Map(e => e.PhoneNumber).Column("PhoneNumberHash");
            Map(e => e.PhoneNumberConfirmed);
            Map(e => e.PasswordHash).Nullable();
            Map(e => e.LockoutEnd).Nullable();
            Map(e => e.AccessFailedCount);
            Map(e => e.LockoutEnabled);
            Map(e => e.LockoutReason);
            HasMany(x => x.Answers).Access.CamelCaseField(Prefix.Underscore).Inverse()
                .Cascade.AllDeleteOrphan();
            HasMany(x => x.UserLogins)
                .Inverse()
                .Cascade.AllDeleteOrphan();

            Map(e => e.TwoFactorEnabled);

            Component(x => x.Transactions, y =>
            {
                y.Map(x => x.Score);
                y.Map(x => x.Balance).CustomSqlType("smallmoney");
                y.HasMany(x => x.Transactions).KeyColumn("User_id")
                    .Inverse() 
                    .Cascade.AllDeleteOrphan().ForeignKeyConstraintName("Transaction_User").AsBag();
            });
            //Map(x => x.Balance).CustomSqlType("smallmoney");
            //Map(x => x.Score);


            Map(x => x.Online);
            Map(x => x.LastOnline);

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Description);

            //HasManyToMany(x => x.Courses)
            //    .ParentKeyColumn("UserId")
            //    .ChildKeyColumn("CourseId")
            //    .Cascade.SaveUpdate()
            //    .ForeignKeyConstraintNames("User_Courses", "Courses_User")
            //    .Table("UsersCourses").AsSet();


            HasMany(x => x.UserCourses)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse().AsSet();


            HasManyToMany(x => x.Tags)
                .ParentKeyColumn("UserId")
                .ChildKeyColumn("TagId")
                .ForeignKeyConstraintNames("User_Tags", "Tags_User")
                .Table("UsersTags").AsSet();


            /*HasMany(x => x.UserRoles)
                .KeyColumn("UserId")
                .Inverse()
                .Cascade.AllDeleteOrphan();*/
            HasOne(x => x.Tutor).Cascade.All();

        }
    }
}