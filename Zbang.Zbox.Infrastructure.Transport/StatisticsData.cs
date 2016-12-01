using ProtoBuf;
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
        public StatisticsData4(StatisticItemData itemId, long userId/*, DateTime statTime*/)
            : this()
        {
            ItemsId = itemId;
            UserId = userId;
           // StatTime = statTime;
        }


        [ProtoMember(3)]
        public IEnumerable<StatisticItemData> ItemsIds { get; private set; }
        [ProtoMember(4)]
        public long UserId { get; private set; }
        [ProtoMember(5)]
        public StatisticItemData ItemsId { get; private set; }


        public override string ProcessResolver => StatisticsResolver;

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
