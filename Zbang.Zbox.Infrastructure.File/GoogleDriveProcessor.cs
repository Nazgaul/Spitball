﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class GoogleDriveProcessor : LinkProcessor
    {
        public GoogleDriveProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {  }
        private const string ContentFormat = "<iframe class=\"iframeContent\" src=\"{0}\"></iframe>";
        public override Task<PreviewResult> ConvertFileToWebSitePreview(Uri contentUrl, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult(new PreviewResult { Content = new List<string> { string.Format(ContentFormat, contentUrl.AbsoluteUri) } });
        }

        public override bool CanProcessFile(Uri contentUrl)
        {
            return contentUrl.AbsoluteUri.ToLower().Contains("google");
        }
        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult<PreProcessFileResult>(null);
        }
        

        

        
    }
}
