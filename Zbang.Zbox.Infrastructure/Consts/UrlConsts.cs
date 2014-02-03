using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Consts
{
    public class UrlConsts
    {
        public const string ItemUrl = "https://www.cloudents.com/item/{0}/{1}";
        public const string BoxUrl = "https://www.cloudents.com/box/{0}";
        public const string PasswordUpdate = "https://www.cloudents.com/account/passwordupdate?key={0}";
        public const string BoxUrlInvite = "https://www.cloudents.com/Share/FromEmail?key={0}&email={1}";
    }

    public class MetaDataConsts
    {
        public const string VideoStatus = "VideoStatus";
    }
}
