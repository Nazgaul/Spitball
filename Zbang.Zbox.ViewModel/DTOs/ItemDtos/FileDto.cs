﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class FileDto : ItemDto
    {
        public FileDto(long id, string name, long ownerId,
            string thumbnail,
            string tabId, int numOfViews, float rate, bool sponsored, string owner,string description)
            : base(id, name, ownerId,
             tabId, numOfViews, rate, thumbnail, sponsored, owner, description)
        {
        }
        public override string Type
        {
            get { return "File"; }
        }
    }
}
