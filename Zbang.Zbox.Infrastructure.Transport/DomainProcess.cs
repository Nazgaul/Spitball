using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    //[ProtoInclude(11, typeof(StatisticsData))]
    //[ProtoInclude(13, typeof(StatisticsData2))]
    [ProtoInclude(14, typeof(StatisticsData3))]
    [ProtoInclude(15, typeof(StatisticsData4))]
    [ProtoInclude(12, typeof(BadItemData))]
    public abstract class DomainProcess
    {
        protected DomainProcess()
        {

        }
        //public const string AddAFriendResolver = "AddAFriend";
        public const string StatisticsResolver = "Statistics";
        public const string BadItemResolver = "BadItem";

        public abstract string ProcessResolver { get; }
    }
}
