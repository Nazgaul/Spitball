using System.Collections.Generic;

namespace Cloudents.Admin2.Models
{
    public class UploadAskFileResponse
    {
        public UploadAskFileResponse(IEnumerable<string> files)
        {
            Files = files;
        }

        public IEnumerable<string> Files { get; set; }
    }
}