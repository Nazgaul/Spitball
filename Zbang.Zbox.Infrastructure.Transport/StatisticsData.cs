using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Transport
{
    //[ProtoContract]
    //public class StatisticsData : DomainProcess
    //{
    //    protected StatisticsData()
    //    {
    //    }
    //    public StatisticsData(IEnumerable<string> itemUids)
    //    {
    //        ItemsUids = itemUids;
    //    }
    //    [ProtoMember(1)]
    //    public IEnumerable<string> ItemsUids { get; private set; }

    //    public override string ProcessResolver
    //    {
    //        get { return DomainProcess.StatisticsResolver; }
    //    }
    //}
    //[ProtoContract]
    //public class StatisticsData2 : DomainProcess
    //{

    //    protected StatisticsData2()
    //    {
    //    }
    //    public StatisticsData2(IEnumerable<StatisticItemData> itemUids)
    //    {
    //        ItemsUids = itemUids;
    //    }


    //    [ProtoMember(2)]
    //    public IEnumerable<StatisticItemData> ItemsUids { get; private set; }

    //    public override string ProcessResolver
    //    {
    //        get { return DomainProcess.StatisticsResolver; }
    //    }

    //    [ProtoContract]
    //    public class StatisticItemData
    //    {
    //        [ProtoMember(1)]
    //        public string Uid { get; set; }
    //        [ProtoMember(2)]
    //        public int Action { get; set; }
    //    }
    //}

    //[ProtoContract]
    //public class StatisticsData3 : DomainProcess
    //{

    //    protected StatisticsData3()
    //    {
    //        ItemsUids = new List<StatisticItemData>();
    //    }
    //    public StatisticsData3(IEnumerable<StatisticItemData> itemUids, long userId, DateTime statTime)
    //        : this()
    //    {
    //        ItemsUids = itemUids;
    //        UserId = userId;
    //        StatTime = statTime;
    //    }


    //    [ProtoMember(3)]
    //    public IEnumerable<StatisticItemData> ItemsUids { get; private set; }
    //    [ProtoMember(4)]
    //    public long UserId { get; private set; }
    //    [ProtoMember(5)]
    //    public DateTime StatTime { get; private set; }


    //    public override string ProcessResolver
    //    {
    //        get { return DomainProcess.StatisticsResolver; }
    //    }

    //    [ProtoContract]
    //    public class StatisticItemData
    //    {
    //        [ProtoMember(1)]
    //        public string Uid { get; set; }
    //        [ProtoMember(2)]
    //        public int Action { get; set; }

    //        [ProtoMember(3)]
    //        public long Id { get; set; }
    //    }
    //}

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
            get { return DomainProcess.StatisticsResolver; }
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
