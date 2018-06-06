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
            Map(e => e.Email).Not.Nullable().Unique();
            Map(e => e.PublicKey);
            Map(e => e.PhoneNumberHash);
            Map(e => e.Name).Not.Nullable().Unique();
            Map(e => e.EmailConfirmed);
            Map(e => e.PhoneNumberConfirmed);
            Map(e => e.NormalizedName);
            Map(e => e.NormalizedEmail);
            Map(e => e.SecurityStamp);
            Map(e => e.Image).Nullable();

            References(x => x.University).ForeignKey("User_University").Nullable();


            /*
             * CREATE UNIQUE NONCLUSTERED INDEX idx_phoneNumber_notnull
               ON sb.[User](PhoneNumberHash)
               WHERE PhoneNumberHash IS NOT NULL;
             */
        }
    }
}
