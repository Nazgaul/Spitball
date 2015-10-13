﻿using System;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class LinkWithDetailDto : ItemWithDetailDto
    {
        public LinkWithDetailDto(long id, DateTime updateTime, string name,

             string userName,
            string userImage,
            long userId, int numberOfViews, string blob, 
            long boxId,
            string boxName, string country, string uniName, string description, string boxUrl)
            : base(id, updateTime, name, userName,
                 userImage,
            userId, numberOfViews, blob,  boxId, boxName, country, uniName, description, boxUrl)
        {

        }

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
