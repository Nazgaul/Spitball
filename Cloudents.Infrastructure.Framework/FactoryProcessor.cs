using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac.Features.Metadata;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public class AppenderMetadata
    {
        public string[] AppenderName { get; set; }
    }
    public class FactoryProcessor
    {
        private readonly IBlobProvider<FilesContainerName> _blobProvider;

        //private readonly IEnumerable<Lazy<IPreviewProvider, AppenderMetadata>> _appenders;
        private readonly IEnumerable<Meta<IPreviewProvider>> _providers;

        public FactoryProcessor(IBlobProvider<FilesContainerName> blobProvider)
        {
            _blobProvider = blobProvider;
        }

        public IPreviewProvider PreviewFactory(string blobName)
        {
            if (!Uri.TryCreate(blobName, UriKind.Absolute, out Uri uri))
            {
                uri = _blobProvider.GetBlobUrl(blobName);
            }

            var process = _providers.FirstOrDefault(f =>
            {
                //if (f.Metadata is string[] p)
                //{
                //    return p.Contains(Path.GetExtension(uri.AbsoluteUri).ToLower());
                //}
                return false;
            });

            return process?.Value;
        }
    }
}
