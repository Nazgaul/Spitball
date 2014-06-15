using System.Globalization;
using Microsoft.WindowsAzure.Storage.Table;


namespace Zbang.Zbox.Infrastructure.Storage.Entities
{
    public class FlagItem : TableEntity
    {

        public FlagItem(long itemId, long userId, string other, string badItem)
            : base("FlagItem", itemId.ToString(CultureInfo.InvariantCulture))
        {

            UserId = userId;
            ItemId = itemId;

            BadItem = badItem;
            Other = other;

        }
        public FlagItem()
        {
        }

        public long UserId { get; set; }

        public long ItemId { get; set; }

        public string BadItem {get;set;}
    
        public string Other { get; set; }
    }
}
