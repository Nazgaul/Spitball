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
            //Map(e => e.CountryCodePhone);
            //Map(e => e.CountryNameIp);

            Map(e => e.Created).Insert().Not.Update();
            Map(e => e.Fictive).ReadOnly();
            Map(e => e.FraudScore);

            Map(e => e.PasswordHash).Nullable();
            Map(e => e.LockoutEnd).Nullable();
            Map(e => e.AccessFailedCount);
            Map(e => e.LockoutEnabled);

            References(x => x.University).ForeignKey("User_University").Nullable();
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

            /*
             * CREATE UNIQUE NONCLUSTERED INDEX idx_phoneNumber_notnull
               ON sb.[User](PhoneNumberHash)
               WHERE PhoneNumberHash IS NOT NULL;
             */
        }
    }

    [UsedImplicitly]
    internal class UserLoginsMap : SpitballClassMap<UserLogin>
    {
        public UserLoginsMap()
        {
            CompositeId().KeyProperty(x => x.LoginProvider).KeyProperty(x => x.ProviderKey);
            Map(x => x.ProviderDisplayName).Nullable();
            References(x => x.User).Column("UserId").ForeignKey("UserLogin_User");
        }
    }
}
