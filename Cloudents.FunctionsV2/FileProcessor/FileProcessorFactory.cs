using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac.Features.Metadata;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.FunctionsV2.FileProcessor
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        readonly IEnumerable<Meta<Lazy<IFileProcessor>>> _providers;

        public FileProcessorFactory(IEnumerable<Meta<Lazy<IFileProcessor>>> appenders)
        {
            _providers = appenders;
        }
        public IFileProcessor GetProcessor(CloudBlockBlob blob)
        {
            var process = _providers.FirstOrDefault(f =>
            {
                var p = (IEnumerable<string>)f.Metadata["AppenderName"] ;

                return p.Contains(Path.GetExtension(blob.Name), StringComparer.OrdinalIgnoreCase);

            });
            return process?.Value.Value;
        }
    }
}