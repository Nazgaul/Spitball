using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [Serializable, DataContract]
    public class ImageProperties
    {
        public enum BlurEffect
        {
            None,
           // Part,
            All
        }
        public ImageProperties(Uri path, BlurEffect blur)
        {
            Path = path.AbsoluteUri;
            Blur = blur;
        }

        public ImageProperties(Uri path)
        {
            Path = path.AbsoluteUri;
        }

        protected ImageProperties()
        {

        }

        [DataMember(Order = 1)]
        public string Path { get; private set; }
        [DataMember(Order = 2)]
        public BlurEffect? Blur { get; private set; }
    }
}