using System;
using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer
{
    [DataContract]
    public class FileProcessQueueData 
    {        
        [DataMember]
        public Uri BlobName { get; set; }
    }


}
