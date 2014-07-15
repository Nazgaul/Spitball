using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IBlobProductProvider
    {
        Task<string> UploadFromLink(byte[] data, string fileName);
    }
}
