using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using System.Collections.Generic;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class UserMap : SubclassMapping<User>
    {
        public UserMap()
        {
            DynamicUpdate(true);
            //DynamicUpdate();
            DiscriminatorValue(false);
            //DiscriminatorValue(false);
            Property(x => x.PhoneNumber, c => c.Column("PhoneNumberHash"));
            //Map(e => e.PhoneNumber).Column("PhoneNumberHash");
            Property(x => x.PhoneNumberConfirmed);
            //Map(e => e.PhoneNumberConfirmed);
            Property(x => x.PasswordHash, c => c.NotNullable(false));
            //Map(e => e.PasswordHash).Nullable();
            Property(x => x.LockoutEnd, c => c.NotNullable(false));
            //Map(e => e.LockoutEnd).Nullable();
            Property(x => x.AccessFailedCount);
            //Map(e => e.AccessFailedCount);
            Property(x => x.LockoutEnabled);
            //Map(e => e.LockoutEnabled);
            Property(x => x.LockoutReason);
            //Map(e => e.LockoutReason);

            Bag(x => x.Answers, c => {
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
                c.Table("Answer");
                c.Key(k => k.Column("UserId"));
            }, a => a.OneToMany());
            //HasMany(x => x.Answers).Access.CamelCaseField(Prefix.Underscore).Inverse()
            //    .Cascade.AllDeleteOrphan();
            Bag(x => x.UserLogins, c => {
                c.Inverse(true);
                c.Key(k => k.Column("UserId"));
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
            }, a => a.OneToMany());
            //HasMany(x => x.UserLogins)
            //    .Inverse()
            //    .Cascade.AllDeleteOrphan();

            Property(x => x.TwoFactorEnabled);
            //Map(e => e.TwoFactorEnabled);

            Component(x => x.Transactions, y =>
            {
                y.Property(x => x.Score);
                y.Property(x => x.Balance, c => c.Column(cl => cl.SqlType("smallmoney")));
                y.Bag(x => x.Transactions, c => {
                    c.Key(k => {
                        k.Column("User_id");
                        k.ForeignKey("Transaction_User");
                        });
                    c.Inverse(true);
                    c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                }, a => a.OneToMany());
                //y.HasMany(x => x.Transactions).KeyColumn("User_id")
                //    .Inverse()
                //    .Cascade.AllDeleteOrphan().ForeignKeyConstraintName("Transaction_User").AsBag();
            });
            //Component(x => x.Transactions, y =>
            //{
            //    y.Map(x => x.Score);
            //    y.Map(x => x.Balance).CustomSqlType("smallmoney");
            //    y.HasMany(x => x.Transactions).KeyColumn("User_id")
            //        .Inverse() 
            //        .Cascade.AllDeleteOrphan().ForeignKeyConstraintName("Transaction_User").AsBag();
            //});
            ////Map(x => x.Balance).CustomSqlType("smallmoney");
            ////Map(x => x.Score);

            Property(x => x.Online);
            //Map(x => x.Online);
            Property(x => x.LastOnline);
            //Map(x => x.LastOnline);
            Property(x => x.FirstName);
            //Map(x => x.FirstName);
            Property(x => x.LastName);
            //Map(x => x.LastName);
            Property(x => x.Description);
            //Map(x => x.Description);

            Component(x => x.BuyerPayment, y =>
            {
                y.Property(z => z.PaymentKey);
                y.Property(z => z.PaymentKeyExpiration);
                y.Property(z => z.CreditCardMask);
            });
            //Component(x => x.BuyerPayment, y =>
            //{
            //    y.Map(z => z.PaymentKey);
            //    y.Map(z => z.PaymentKeyExpiration);
            //    y.Map(z => z.CreditCardMask);
            //});
            Property(x => x.PaymentExists);
            //Map(z => z.PaymentExists).CustomType<PaymentStatus>();
            Property(x => x.Gender);
            //Map(z => z.Gender).CustomType<Gender>();
            ////HasManyToMany(x => x.Courses)
            ////    .ParentKeyColumn("UserId")
            ////    .ChildKeyColumn("CourseId")
            ////    .Cascade.SaveUpdate()
            ////    .ForeignKeyConstraintNames("User_Courses", "Courses_User")
            ////    .Table("UsersCourses").AsSet();

            //Set<UserCourse>("_userCourses", c => {
            //    c.Cascade(Cascade.All | Cascade.DeleteOrphans);
            //    c.Inverse(true);
            //    c.Table("UsersCourses");
            //    c.Key(k =>
            //    {
            //        k.Columns(cl => cl.Name("UserId"));
            //        k.ForeignKey("User_Courses");
            //    });
            //    c.Access(Accessor.NoSetter);
            //}, a => a.OneToMany());

            Set(x => x.UserCourses, c => {
                c.Key(k =>
                {
                    k.Column("CourseId");
                    k.ForeignKey("Courses_User");
                });
                c.Inverse(true);
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Table("UsersCourses");
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
            }, a => a.OneToMany(/*x => x.Class(typeof(UserCourse))*/)
           );

            //Property(x => x.UserCourses, c =>
            //{
            //    c.Access(Accessor.NoSetter);
            //    //c.Access(Accessor.ReadOnly);
            //});
            //HasMany(x => x.UserCourses).Access.CamelCaseField(Prefix.Underscore)
            //    .Cascade.AllDeleteOrphan()
            //    .KeyColumn("UserId").Inverse().AsSet();

            Bag(x => x.StudyRooms, c => {
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Inverse(true);
                c.Key(k => k.Column("UserId"));
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
            }, a => a.OneToMany(o => { }));
            //HasMany(x => x.StudyRooms).Access.CamelCaseField(Prefix.Underscore)
            //    .Cascade.AllDeleteOrphan()
            //    .KeyColumn("UserId").Inverse();
            ////StudyRooms

            Set(x => x.Tags, c => {
                c.Table("UsersTags");
                c.Key(k => {
                    k.Column("UserId");
                    k.ForeignKey("User_Tags");
                    k.ForeignKey("Tags_User");
                });
            }, m => m.ManyToMany(p => p.Column("TagId")));
            //HasManyToMany(x => x.Tags)
            //    .ParentKeyColumn("UserId")
            //    .ChildKeyColumn("TagId")
            //    .ForeignKeyConstraintNames("User_Tags", "Tags_User")
            //    .Table("UsersTags").AsSet();



            ///*HasMany(x => x.UserRoles)
            //    .KeyColumn("UserId")
            //    .Inverse()
            //    .Cascade.AllDeleteOrphan();*/
            OneToOne(x => x.Tutor, c => c.Cascade(Cascade.All));
            //HasOne(x => x.Tutor)/*.LazyLoad(Laziness.NoProxy).Constrained()*/.Cascade.All();

        }
    }
}