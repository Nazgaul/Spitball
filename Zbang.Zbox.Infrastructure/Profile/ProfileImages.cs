using System;

namespace Zbang.Zbox.Infrastructure.Profile
{
    [Serializable]
    public class ProfileImages
    {
        public ProfileImages()
        {
        }
        public ProfileImages(string image)
        {
            Uri temp;
            if (Uri.TryCreate(image, UriKind.Absolute, out temp))
            {
                Image = temp;
            }
            
            //Image = image;
            //LargeImage = largeImage;
        }
        public ProfileImages(Uri image)
        {
            Image = image;
            //LargeImage = largeImage;
        }
        public Uri Image { get; set; }
       // public Uri LargeImage { get; set; }
    }
}
