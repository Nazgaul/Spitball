using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public interface IPreviewProvider2
    {
        //Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(int indexNum,
        //    CancellationToken cancelToken);


        Task CreatePreviewFilesAsync(MemoryStream stream, Func<Stream,string,Task> callback, Func<string, Task> textCallback, CancellationToken token);
    }
}
