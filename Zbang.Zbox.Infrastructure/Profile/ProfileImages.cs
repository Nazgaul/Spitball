using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Profile
{
    [Serializable]
    public class ProfileImages
    {
        public ProfileImages()
        {
        }
        public ProfileImages(string image, string largeImage)
        {
            Uri temp;
            if (Uri.TryCreate(image, UriKind.Absolute, out temp))
            {
                Image = temp;
            }
            if (Uri.TryCreate(largeImage, UriKind.Absolute, out temp))
            {
                LargeImage = temp;
            }
            
            //Image = image;
            //LargeImage = largeImage;
        }
        public ProfileImages(Uri image, Uri largeImage)
        {
            Image = image;
            LargeImage = largeImage;
        }
        public Uri Image { get; set; }
        public Uri LargeImage { get; set; }
    }
}
