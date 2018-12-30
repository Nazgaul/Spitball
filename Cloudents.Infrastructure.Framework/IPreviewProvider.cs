using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public interface IPreviewProvider2
    {
        /// <summary>
        /// Process file upload in the system
        /// </summary>
        /// <param name="stream">The stream of the file</param>
        /// <param name="pagePreviewCallback">image per page upload, arg1 the image stream, arg 2 file name</param>
        /// <param name="metaCallback">meta to add to the document, arg1 the text extracted arg2 the number of document</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task ProcessFilesAsync(Stream stream,
            Func<Stream, string, Task> pagePreviewCallback,
            Func<string, int, Task> metaCallback,
            CancellationToken token);
    }

    public interface IBlurProcessor
    {
        Task ProcessBlurPreviewAsync(Stream stream, bool firstPage,
            Func<Stream, Task> pagePreviewCallback,
            CancellationToken token);
    }
}
