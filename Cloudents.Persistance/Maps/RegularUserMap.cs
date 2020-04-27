using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class UserMap : SubclassMap<User>
    {
        public UserMap()
        {

            DynamicUpdate();
            DiscriminatorValue(false);
            Map(e => e.PhoneNumber).Column("PhoneNumberHash");
            Map(e => e.PhoneNumberConfirmed);
            Map(e => e.PasswordHash).Nullable();
            Map(e => e.LockoutEnd).Nullable();
            Map(e => e.AccessFailedCount);
            Map(e => e.LockoutEnabled);
            Map(e => e.LockoutReason);
            HasMany(x => x.Answers).Inverse()
                .Cascade.AllDeleteOrphan();
            HasMany(x => x.UserLogins)
                .Inverse()
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.UserLocations)
                .Inverse()
                .Cascade.AllDeleteOrphan();

            //Map(e => e.TwoFactorEnabled);

            Component(x => x.Transactions, y =>
            {
                //y.Map(x => x.Score);
                y.Map(x => x.Balance).CustomSqlType("smallmoney");
                y.HasMany(x => x.Transactions).KeyColumn("User_id")
                    .Inverse()
                    .Cascade.AllDeleteOrphan().ForeignKeyConstraintName("Transaction_User").AsBag();
            });


            Map(x => x.Online);
            Map(x => x.LastOnline);
            Map(x => x.FinishRegistrationDate).Column("FinishRegistration");

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Description);
            Map(x => x.CoverImage);

            Component(x => x.BuyerPayment, y =>
            {
                y.Map(z => z!.PaymentKey);
                y.Map(z => z!.PaymentKeyExpiration);
                y.Map(z => z!.CreditCardMask);

            });
            Map(z => z.PaymentExists).CustomType<PaymentStatus>();
            Map(z => z.Gender).CustomType<Gender>().Nullable();
            //Map(x => x.UserType2).Column("UserType").CustomType<GenericEnumStringType<UserType>>().Nullable();
            HasMany(x => x.UserCourses).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse().AsSet();

            HasMany(x => x.ChatUsers).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse();

            HasOne(x => x.Tutor).Cascade.None();
            HasMany(x => x.UserCoupon).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse().AsSet();


            HasMany(x => x.StudyRooms).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse();

            //We are using cascade all because we need to save the tutor in Become Tutor command handler

            //HasMany(x => x.UserComponents).Inverse().Cascade.AllDeleteOrphan();//.Inverse();

            HasMany(x => x.UserTokens)
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();


            HasMany(x => x.Followers).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse().AsSet();


            HasMany(x => x.Followed).KeyColumn("FollowerId")
                .Cascade.AllDeleteOrphan().AsSet();

            HasMany(x => x.Leads)
                .Cascade.AllDeleteOrphan();
        }
    }




    //public class UserComponentMap : ClassMap<UserComponent>
    //{
    //    public UserComponentMap()
    //    {
    //        Table("UserExtension");
            
    //        Id(x => x.Id).GeneratedBy.GuidComb();
    //        Map(x => x.Type).ReadOnly().Not.Nullable().Insert().Not.Update();
    //        References(x => x.User).Not.Nullable();
    //    }
    //}

    //public class ParentMap : SubclassMap<Parent>
    //{
    //    public ParentMap()
    //    {
    //        Table("UserParent");

    //        Map(x => x.Name).Column("ChildFirstName").Nullable();
    //        Map(x => x.Grade).Nullable();
    //    }
    //}

    //public class HighSchoolStudentMap : SubclassMap<HighSchoolStudent>
    //{
    //    public HighSchoolStudentMap()
    //    {
    //        Table("UserHighSchool");

    //        //Map(x => x.ChildFirstName).Nullable();
    //        //Map(x => x.ChildLastName).Nullable();
    //        //Map(x => x.Grade).Nullable();
    //    }
    //}

    //public class CollegeStudentMap : SubclassMap<CollegeStudent>
    //{
    //    public CollegeStudentMap()
    //    {
    //        Table("UserCollege");

    //        //Map(x => x.ChildFirstName).Nullable();
    //        //Map(x => x.ChildLastName).Nullable();
    //        //Map(x => x.Grade).Nullable();
    //    }
    //}


    //public class TeacherMap : SubclassMap<Teacher>
    //{
    //    public TeacherMap()
    //    {
    //        Table("UserTeacher");

    //        //Map(x => x.ChildFirstName).Nullable();
    //        //Map(x => x.ChildLastName).Nullable();
    //        //Map(x => x.Grade).Nullable();
    //    }
    //}
}