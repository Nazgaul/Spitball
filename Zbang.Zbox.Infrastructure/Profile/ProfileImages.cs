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
            if (Uri.TryCreate(image, UriKind.Absolute, out Uri temp))
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
