using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    [ProtoInclude(15, typeof(StatisticsData4))]
    [ProtoInclude(12, typeof(BadItemData))]
    [ProtoInclude(16, typeof(UpdateData))]
    [ProtoInclude(17, typeof(UniversityData))]
    [ProtoInclude(18, typeof(ReputationData))]
    [ProtoInclude(19, typeof(QuotaData))]
    [ProtoInclude(20, typeof(DeleteBoxData))]
    [ProtoInclude(21, typeof(BadPostData))]
    [ProtoInclude(22, typeof(NewUserData))]
    [ProtoInclude(23, typeof(BadgeData))]
    public abstract class DomainProcess
    {
        // ReSharper disable once EmptyConstructor - for protobuf
        protected DomainProcess()
        { }
        public const string StatisticsResolver = "Statistics";
        public const string BadItemResolver = "BadItem";

        public const string UpdateResolver = "UpdateResolver";
        public const string UniversityResolver = "University";
        public const string ReputationResolver = "Reputation";
        public const string BadgeResolver = "Badge";
        public const string DeleteBoxResolver = "DeleteBox";
        public const string QuotaResolver = "Quota";

        public const string UserResolver = "User";
        public abstract string ProcessResolver { get; }
    }
}
