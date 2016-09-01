using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class UniversityProcessData : FileProcess
    {
        protected UniversityProcessData()
        {
        }

        public UniversityProcessData(long universityId)
        {
            UniversityId = universityId;
        }

        [ProtoMember(1)]
        public long UniversityId { get; private set; }
        public override string ProcessResolver => nameof(UniversityProcessData);
    }
}