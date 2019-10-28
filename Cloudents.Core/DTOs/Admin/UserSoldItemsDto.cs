using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserSoldItemsDto
    {
        [EntityBind(nameof(Transaction.Created))]
        public DateTime TransactionTime { get; set; }
        [EntityBind(nameof(Transaction.Price))]
        public decimal TransactionPrice { get; set; }
        [EntityBind(nameof(Document.Id))]
        public long ItemId { get; set; }
        [EntityBind(nameof(Document.Name))]
        public string ItemName{ get; set; }
        [EntityBind(nameof(Document.TimeStamp.CreationTime))]
        public DateTime ItemCreated { get; set; }
        [EntityBind(nameof(Document.Course))]
        public string ItemCourse { get; set; }
        [EntityBind(nameof(Document.Status.State))]
        public ItemState ItemState { get; set; }
        [EntityBind(nameof(Document.DocumentType))]
        public DocumentType? ItemType { get; set; }
        [EntityBind(nameof(User.Name))]
        public string PurchasedUserName { get; set; }
        [EntityBind(nameof(User.Email))]
        public string PurchasedUserEmail { get; set; }
        [EntityBind(nameof(User.Balance))]
        public decimal PurchasedUserBalance { get; set; }
        public string Url{ get; set; }
    }
}
