using Cloudents.Core;

namespace Cloudents.FunctionsV2.Models
{
    public class Star : Enumeration
    {
        public string BlobPath { get; }
        private Star(int id, string name, string blobPath) : base(id, name)
        {
            BlobPath = $"share-placeholder/{blobPath}";

        }

        public static readonly Star Full = new Star(1, "Full", "star-full.png");
        public static readonly Star Half = new Star(2, "Half", "star-half.png");
        public static readonly Star None = new Star(3, "Empty", "star-empty.png");

    }
}