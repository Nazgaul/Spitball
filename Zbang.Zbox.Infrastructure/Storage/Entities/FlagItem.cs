using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Storage.Entities
{
    public class FlagItem : TableEntity
    {

        public FlagItem(long itemId, long userId, string other, string badItem)
            : base("FlagItem", itemId.ToString())
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
