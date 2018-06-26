using System;
using System.Reflection.Emit;
using System.Threading;
using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data.Maps
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
           // Map(e => e.Balance).CustomSqlType("smallmoney");

            References(x => x.University).ForeignKey("User_University").Nullable();

            References(x => x.LastTransaction).ForeignKey("User_Transaction");//.Cascade.SaveUpdate();

            HasMany(x => x.Transactions)
                .Inverse()

                //.Inverse()
                //TODO: this is generate exception when creating new answer. need to figure it out
                //    .Not.KeyNullable()
                //    .Not.KeyUpdate()
                .Cascade.AllDeleteOrphan();

            /*
             * CREATE UNIQUE NONCLUSTERED INDEX idx_phoneNumber_notnull
               ON sb.[User](PhoneNumberHash)
               WHERE PhoneNumberHash IS NOT NULL;
             */
        }
    }
}
