using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer
{
    [DataContract]
    [KnownType(typeof(GenerateThumbnail))]
    public abstract class MailQueueData
    {
        protected MailQueueData(string resolverName)
        {
            ResolverName = resolverName;
        }

        [DataMember]
        public string ResolverName
        {
            get;
            protected set;
        }

    }

}
