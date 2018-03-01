using Cloudents.Core.Attributes;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    [Db(Database.MailGun)]
    public class MailGunStudent
    {
        public virtual int Id { get; set; }
        public virtual string Sent { get; set; }

        public virtual bool ShouldSend { get; set; }
    }
}