using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserSoldItemsDto
    {
        public DateTime TransactionTime { get; set; }
        public decimal TransactionPrice { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public DateTime ItemCreated { get; set; }
        public string ItemCourse { get; set; }
        public ItemState ItemState { get; set; }
        public DocumentType? ItemType { get; set; }
        public string PurchasedUserName { get; set; }
        public string PurchasedUserEmail { get; set; }
        public decimal PurchasedUserBalance { get; set; }
        public string Url { get; set; }
    }
}
