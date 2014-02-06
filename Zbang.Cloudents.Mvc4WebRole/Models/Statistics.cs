using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class Statistics
    {

        public StatisticItem[] Items { get; set; }

        public override string ToString()
        {
            if (Items == null)
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            foreach (var item in Items)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();

        }
    }
    public class StatisticItem
    {
        public long Uid { get; set; }
        public Zbang.Zbox.Infrastructure.Enums.StatisticsAction Action { get; set; }

        public override string ToString()
        {
            return string.Format("Uid: {0} Action:{1}", Uid, Action);
        }
    }
}