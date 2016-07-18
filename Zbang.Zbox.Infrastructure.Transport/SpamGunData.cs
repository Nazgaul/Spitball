using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class SpamGunData
    {
        [ProtoMember(1)]
        public string Emails { get; set; }
    }
}
