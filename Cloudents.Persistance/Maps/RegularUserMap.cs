using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
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
            HasMany(x => x.Answers)/*.Access.CamelCaseField(Prefix.Underscore)*/.Inverse()
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

            Component(x => x.BuyerPayment, y =>
            {
                y.Map(z => z.PaymentKey);
                y.Map(z => z.PaymentKeyExpiration);
                y.Map(z => z.CreditCardMask);

            });
            Map(z => z.PaymentExists).CustomType<PaymentStatus>();
            Map(z => z.Gender).CustomType<Gender>();


            HasMany(x => x.UserCourses)/*.Access.CamelCaseField(Prefix.Underscore)*/
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse().AsSet();


            HasMany(x => x.UserCoupon)/*.Access.CamelCaseField(Prefix.Underscore)*/
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse().AsSet();


            HasMany(x => x.StudyRooms).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .KeyColumn("UserId").Inverse();

            //We are using cascade all because we need to save the tutor in Become Tutor command handler
            HasOne(x => x.Tutor).Cascade.All();

        }
    }
}