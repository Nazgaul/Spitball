﻿using System;
using System.IO;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class FileWithDetailDto : ItemWithDetailDto
    {

        public FileWithDetailDto(long id, DateTime updateTime, string name,
            string userName,
            string userImage,
            string blob, long userId, int numberOfViews, int numberOfDownloads,  long boxId, string boxName,
            string country, string uniName, string description, string boxUrl)
            : base(id, updateTime, name, userName,
                userImage,
            userId, numberOfViews, blob,  boxId, boxName, country, uniName, description, boxUrl)
        {
            //TODO: this is not good should be logic in dto
            NameWOExtension = Path.GetFileNameWithoutExtension(Name);
            NDownloads = numberOfDownloads;
        }
        public string NameWOExtension { get; set; }

        public int NDownloads { get; private set; }

        public override string Type
        {
            get { return "File"; }
        }
    }
}
