using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac.Features.Metadata;

namespace Cloudents.Infrastructure.Framework
{
    public class FileFactoryProcessor : IFactoryProcessor
    {

        private readonly IEnumerable<Meta<Func<string, IPreviewProvider2>>> _providers;

        public FileFactoryProcessor(
            IEnumerable<Meta<Func<string, IPreviewProvider2>>> providers)
        {
            _providers = providers;
        }

        public IPreviewProvider2 PreviewFactory(string blobName)
        {
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
