using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    [ProtoInclude(15, typeof(StatisticsData4))]
    [ProtoInclude(12, typeof(BadItemData))]
    [ProtoInclude(16, typeof(UpdateData))]
    public abstract class DomainProcess
    {
// ReSharper disable once EmptyConstructor - for protobuf
        protected DomainProcess()
        {

        }
        //public const string AddAFriendResolver = "AddAFriend";
        public const string StatisticsResolver = "Statistics";
        public const string BadItemResolver = "BadItem";
        public const string UpdateResolver = "UpdateResolver";

        public abstract string ProcessResolver { get; }
    }
}
