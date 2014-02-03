using System;
using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Zbox.ViewModel.DTOs
{
    public abstract class ItemDto
    {
        public string Name { get; set; }

        public string UploaderName { get; set; }
        public string UploaderImage { get; set; }

        public long Size { get; set; }

        public DateTime CreationTime { get; set; }

        public string BoxName { get; set; }


        public abstract string ItemType { get; protected set; }

        public string UserUid { get; set; }
        public string Uid { get; set; }

        public string BoxUid { get; set; }
        public DateTime UpdateTime { get; set; }

        public bool Deleted { get; private set; }
    }
}
