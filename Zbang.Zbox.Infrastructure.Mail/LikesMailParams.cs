using System.Collections.Generic;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class LikesMailParams : MailParameters
    {
        public LikesMailParams(CultureInfo info, string name, IEnumerable<LikeData> likeData)
            : base(info)
        {
            Name = name;
            LikeData = likeData;
        }

        public string Name { get; private set; }

        public IEnumerable<LikeData> LikeData { get; private set; }

        public override string MailResover => nameof(LikesMailParams);
    }

    public class LikeData
    {
        public string UserName { get; set; }
        public LikeType Type { get; set; }

        public string OnLikeText { get; set; }
    }
}
