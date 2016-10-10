using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public class PreProcessFileResult
    {
        public string BlobName { get; set; }
       // public string ThumbnailName { get; set; }

        //public string FileTextContent { get; set; }

        public static readonly Task<PreProcessFileResult> GetEmptyResult = Task.FromResult<PreProcessFileResult>(null);

    }
}
