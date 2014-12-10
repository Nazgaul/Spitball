using System.Text;

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
        public Zbox.Infrastructure.Enums.StatisticsAction Action { get; set; }

        public override string ToString()
        {
            return string.Format("Uid: {0} Action:{1}", Uid, Action);
        }
    }
}