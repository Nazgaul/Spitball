using System;
using System.Runtime.Serialization;

namespace Zbang.Zbox.WcfRestService
{
    [DataContract]
    public class UserToken
    {
        [DataMember]
        public long UserId { get; set; }
        [DataMember]
        public DateTime ExpireTokenTime { get; set; }
    }
}