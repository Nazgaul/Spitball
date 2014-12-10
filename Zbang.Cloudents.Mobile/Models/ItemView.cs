using System;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class ItemView
    {
        public ItemView(ItemWithDetailDto dto)
        {
            Uid = dto.Uid;
            UpdateTime = dto.UpdateTime;
            Name = dto.Name;
            //BoxName = dto.BoxName;
            //BoxUid = dto.BoxUid;
            Owner = dto.Owner;
            OwnerImg = dto.OwnerImg;
            OwnerUid = dto.OwnerUid;
            Type = dto.Type;
            UserType = dto.UserType;
            NumberOfViews = dto.NumberOfViews;
        }

        public ItemView(ItemWithDetailDto dto, string preview)
            :this(dto)
        {
            Preview = preview;
        }
        public string Uid { get; private set; }
        public string Name { get; private set; }
       // public string BoxName { get; private set; }
       // public string BoxUid { get; private set; }
        public DateTime UpdateTime { get; private set; }
        public string Owner { get; private set; }
        public string OwnerImg { get; private set; }
        public string OwnerUid { get; private set; }
        public string Type { get; private set; }
        public string Preview { get; set; }
        public bool Like { get; private set; }
        public int LikeCount { get; private set; }
        public int  NumberOfViews { get; private set; }

        public Zbox.Infrastructure.Enums.UserRelationshipType UserType { get; private set; }
    }
}