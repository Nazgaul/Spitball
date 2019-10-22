using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserSoldItemsDto
    {
        [EntityBind(nameof(Transaction.Id))]
        public Guid TxId { get; set; }  
        [EntityBind(nameof(Transaction.Created))]
        public DateTime TxCreated { get; set; }
        [EntityBind(nameof(Transaction.Price))]
        public decimal TxPrice { get; set; }
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
        public string UserName{ get; set; }
        [EntityBind(nameof(User.Email))]
        public string UserEmail { get; set; }
        [EntityBind(nameof(User.Balance))]
        public decimal UserBalance { get; set; }
    }
}
