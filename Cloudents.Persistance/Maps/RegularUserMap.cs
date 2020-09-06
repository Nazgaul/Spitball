using System;
using System.Globalization;
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
          

            HasMany(x => x.UserLogins)
                .Inverse()
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.UserLocations)
                .Inverse()
                .Cascade.AllDeleteOrphan();


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

            Map(x => x.LastName);
            Map(x => x.CoverImage);

          

            ReferencesAny(x => x.Payment)
                .MetaType<string>()
                .IdentityType<Guid>()
                .EntityIdentifierColumn("PaymentId")
                .EntityTypeColumn("PaymentType")
                .AddMetaValue<PaymePayment>("Payme")
                .AddMetaValue<StripePayment>("Stripe").Cascade.All();

            Map(z => z.PaymentExists).CustomType<PaymentStatus>();
         

            HasMany(x => x.ChatUsers).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse();

            //On sms api - we got  No persister for: UserProxyForFieldInterceptor if we put no Proxy on lazy type
            //on create user we need to create tutor
            HasOne(x => x.Tutor).Cascade.All()/*.LazyLoad(Laziness.NoProxy)*/;

            HasMany(x => x.UserCoupon).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse().AsSet();


            HasMany(x => x.StudyRooms).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse();


            HasMany(x => x.DocumentDownloads)
                .KeyColumn("UserId").Cascade.AllDeleteOrphan()
                .Inverse();

            HasMany(x => x.SessionPayments)
                .KeyColumn("UserId").Cascade.AllDeleteOrphan()
                .Inverse();

            HasMany(x => x.StudyRoomSessionUsers)
                .KeyColumn("UserId").Cascade.AllDeleteOrphan()
                .Inverse();

            //We are using cascade all because we need to save the tutor in Become Tutor command handler
            //HasMany(x => x.UserComponents).Inverse().Cascade.AllDeleteOrphan();//.Inverse();


            HasMany(x => x.Followers).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse().AsSet();


            HasMany(x => x.Following).KeyColumn("FollowerId")
                .Cascade.AllDeleteOrphan().AsSet();

            HasMany(x => x.Leads)
                .Cascade.AllDeleteOrphan();

           // Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='User'");
           // Map(e => e.Email)/*.Not.Nullable()*/.Unique();
          //  Map(e => e.Name).Not.Nullable();
            Map(e => e.EmailConfirmed);

            Map(e => e.SecurityStamp);
            Map(e => e.AuthenticatorKey);
            // Map(e => e.Culture);



            Map(e => e.Country).Nullable().Length(2);
            Map(e => e.SbCountry).CustomType<EnumerationType<Country>>().Nullable();

            Map(e => e.Created).Insert().Not.Update();



            Map(x => x.Language).Column("Language").CustomType<CultureInfo>().Nullable();

            HasMany(x => x.UserCourses).Access.CamelCaseField(Prefix.Underscore).KeyColumn("UserId")
                .Inverse()
                .Cascade.AllDeleteOrphan();


            HasMany(x => x.Documents).Access.CamelCaseField(Prefix.Underscore).KeyColumn("UserId")
                .Inverse()
                .Cascade.AllDeleteOrphan();


         

            Table("User"); //if not there is sql error

            DynamicUpdate();
           // OptimisticLock.Version();
           // Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
        }
    }

    public class PaymePaymentMap : ClassMap<PaymePayment>
    {
        public PaymePaymentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(z => z!.PaymentKey);
            Map(z => z!.PaymentKeyExpiration);
            Map(z => z!.CreditCardMask);
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