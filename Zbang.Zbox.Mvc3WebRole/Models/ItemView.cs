using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class ItemView
    {
        public ItemView(ItemWithDetailDto dto, string preview)
        {
            Uid = dto.Uid;
            UpdateTime = dto.UpdateTime;
            Name = dto.Name;
            BoxName = dto.BoxName;
            BoxUid = dto.BoxUid;
            Owner = dto.Owner;
            OwnerImg = dto.OwnerImg;
            OwnerUid = dto.OwnerUid;
            Preview = preview;
            Type = dto.Type;

        }
        public string Uid { get; private set; }
        public string Name { get; private set; }
        public string BoxName { get; private set; }
        public string BoxUid { get; private set; }
        public DateTime UpdateTime { get; private set; }
        public string Owner { get; private set; }
        public string OwnerImg { get; private set; }
        public string OwnerUid { get; private set; }
        public string Type { get; private set; }
        public string Preview { get; private set; }
    }
}