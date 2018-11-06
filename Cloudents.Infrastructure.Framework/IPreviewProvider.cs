using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public interface IPreviewProvider2
    {
        Task ProcessFilesAsync(Stream stream,
            Func<Stream,string,Task> pagePreviewCallback,
            Func<string, Task> textCallback,
            Func<int, Task> pageCountCallback,
            CancellationToken token);
    }
}
