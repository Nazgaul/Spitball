using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
     [ProtoContract]
    public class UniversityData : DomainProcess
    {
        protected UniversityData()
        {
            
        }
        public UniversityData(string name, long id, string image)
        {
            Image = image;
            Id = id;
            Name = name;
        }
          [ProtoMember(1)]
        public string Image { get; private set; }
          [ProtoMember(2)]
        public string Name { get; private set; }
          [ProtoMember(3)]
        public long Id { get;private set; }

        public override string ProcessResolver
        {
            get { return UniversityResolver; }
        }
    }
}