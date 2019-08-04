﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public interface IPreviewProvider2
    {

        void Init(Func<Stream> stream);
        void Init(Func<string> file);
       // void Init(Func<string> path);
        (string text, int pagesCount) ExtractMetaContent();

        /// <summary>
        /// Process file upload in the system
        /// </summary>
        /// <param name="previewDelta"></param>
        /// <param name="pagePreviewCallback">image per page upload, arg1 the image stream, arg 2 file name</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task ProcessFilesAsync(IEnumerable<int> previewDelta,
            Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token);

    }

    //public interface IBlurProcessor
    //{
    //    Task ProcessBlurPreviewAsync(Stream stream, bool firstPage,
    //        Func<Stream, Task> pagePreviewCallback,
    //        CancellationToken token);
    //}
}
