using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class LinkWithDetailDto : ItemWithDetailDto
    {
        public LinkWithDetailDto(long id, DateTime updateTime, string name,

             string userName,
            string userImage,
            long userId, int numberOfViews, string blob, float rate,
            long boxId,
            string boxName, string country, string uniName)
            : base(id, updateTime, name, userName,
                 userImage,
            userId, numberOfViews, blob, rate, boxId, boxName, country, uniName)
        {

        }

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
