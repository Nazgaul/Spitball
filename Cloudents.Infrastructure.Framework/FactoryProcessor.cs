using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac.Features.Metadata;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public class FileFactoryProcessor : IFactoryProcessor
    {

        private readonly IEnumerable<Meta<Func<string, IPreviewProvider>>> _providers;

        public FileFactoryProcessor(
            IEnumerable<Meta<Func<string, IPreviewProvider>>> providers)
        {
            _providers = providers;
        }

        public IPreviewProvider PreviewFactory(string blobName)
        {
            //if (!Uri.TryCreate(blobName, UriKind.Absolute, out var uri))
            //{
            //    uri = _blobProvider.GetBlobUrl(blobName);
            //}

            var process = _providers.FirstOrDefault(f =>
            {
                if (f.Metadata["AppenderName"] is string[] p)
                {
                    return p.Contains(Path.GetExtension(blobName), StringComparer.OrdinalIgnoreCase);
                }
                return false;
            });
            return process?.Value(blobName);
        }
    }
}
