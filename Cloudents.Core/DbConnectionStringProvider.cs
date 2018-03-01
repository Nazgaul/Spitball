using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core
{
    [UsedImplicitly]
    public class DbConnectionStringProvider
    {
        private readonly IConfigurationKeys _keys;

        public DbConnectionStringProvider(IConfigurationKeys keys)
        {
            _keys = keys;
        }

        public string GetConnectionString(Database db)
        {
            if (db == Database.MailGun)
            {
                return _keys.MailGunDb;
            }
            else
            {
                return _keys.Db;
            }
        }
    }
}