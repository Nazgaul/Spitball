
namespace Zbang.Zbox.WcfRestService.Models
{
    public class AddFileToBox
    {
        
        public string FileName { get; set; }
        public string BlobName { get; set; }

        public override string ToString()
        {
            return string.Format("  FileName {0} BlobName {1}",
                   FileName, BlobName);
        }
    }
}