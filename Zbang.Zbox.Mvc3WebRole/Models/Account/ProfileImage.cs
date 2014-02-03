using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Zbox.Mvc3WebRole.Models.Account
{
    [Serializable]
    public class ProfileImage
    {
        public ProfileImage()
        {
        }
        public ProfileImage(Uri image, Uri largeImage)
        {
            Image = image;
            LargeImage = largeImage;
        }
        public Uri Image { get; set; }
        public Uri LargeImage { get; set; }
    }
}