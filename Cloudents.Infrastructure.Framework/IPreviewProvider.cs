using Aspose.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public interface IPreviewProvider2
    {

        void Init(Stream stream);
        (string text, int pagesCount) ExtractMetaContent();
        int ExtractPagesCount();
        /// <summary>
        /// Process file upload in the system
        /// </summary>
        /// <param name="stream">The stream of the file</param>
        /// <param name="pagePreviewCallback">image per page upload, arg1 the image stream, arg 2 file name</param>
        /// <param name="metaCallback">meta to add to the document, arg1 the text extracted arg2 the number of document</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task ProcessFilesAsync(List<int> previewDelta,
            Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token);

    }

    public interface IBlurProcessor
    {
        Task ProcessBlurPreviewAsync(Stream stream, bool firstPage,
            Func<Stream, Task> pagePreviewCallback,
            CancellationToken token);
    }
}
