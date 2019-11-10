using Autofac.Features.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cloudents.Infrastructure.Framework
{
    public class FileFactoryProcessor : IFactoryProcessor
    {

        readonly IEnumerable<Meta<Lazy<IPreviewProvider>>> _providers;

        public FileFactoryProcessor(IEnumerable<Meta<Lazy<IPreviewProvider>>> appenders)
        {
            _providers = appenders;
        }


        //private readonly IEnumerable<Meta<Func<string, IPreviewProvider>>> _providers;

        //public FileFactoryProcessor(
        //    IEnumerable<Meta<Func<string, IPreviewProvider>>> providers)
        //{
        //    _providers = providers;
        //}

        public IPreviewProvider PreviewFactory(string blobName)
        {
            var process = _providers.FirstOrDefault(f =>
            {
                if (f.Metadata["AppenderName"] is string[] p)
                {
                    return p.Contains(Path.GetExtension(blobName), StringComparer.OrdinalIgnoreCase);
                }
                return false;
            });
            return process?.Value.Value;
            //return process?.Value(blobName);
        }
    }
}
