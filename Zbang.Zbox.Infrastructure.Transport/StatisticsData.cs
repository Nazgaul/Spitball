using ProtoBuf;
using System;
using System.Collections.Generic;


namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class StatisticsData4 : DomainProcess
    {

        protected StatisticsData4()
        {
            ItemsIds = new List<StatisticItemData>();
        }
        public StatisticsData4(IEnumerable<StatisticItemData> itemIds, long userId, DateTime statTime)
            : this()
        {
            ItemsIds = itemIds;
            UserId = userId;
            StatTime = statTime;
        }


        [ProtoMember(3)]
        public IEnumerable<StatisticItemData> ItemsIds { get; private set; }
        [ProtoMember(4)]
        public long UserId { get; private set; }
        [ProtoMember(5)]
        public DateTime StatTime { get; private set; }


        public override string ProcessResolver
        {
            get { return StatisticsResolver; }
        }

        [ProtoContract]
        public class StatisticItemData
        {
           
            [ProtoMember(2)]
            public int Action { get; set; }

            [ProtoMember(3)]
            public long Id { get; set; }
        }
    }
}
