using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.StorageApp
{
    public interface IBlobUpload
    {
        string GenerateWriteAccessPermissionToBlob(string blobName, string mimeType);
    }
}
