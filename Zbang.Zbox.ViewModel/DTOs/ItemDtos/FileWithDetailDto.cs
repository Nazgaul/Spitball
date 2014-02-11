using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class FileWithDetailDto : ItemWithDetailDto
    {

        public FileWithDetailDto(long id, DateTime updateTime, string name,
            string userName,
            string userImage,
            string blob, long userId, int numberOfViews, int numberOfDownloads, float rate, long boxId, string boxName,
            string country, string uniName, string description)
            : base(id, updateTime, name, userName,
                userImage,
            userId, numberOfViews, blob, rate, boxId, boxName, country, uniName, description)
        {
            //TODO: this is not good should be logic in dto
            NameWOExtension = Path.GetFileNameWithoutExtension(Name);
            Blob = BlobProvider.GetBlobUrl(blob);
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
