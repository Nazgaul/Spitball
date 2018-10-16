﻿using System;
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
        private readonly IBlobProvider<OldSbFilesContainerName> _blobProvider;

        private readonly IEnumerable<Meta<Func<Uri, IPreviewProvider>>> _providers;

        public FileFactoryProcessor(IBlobProvider<OldSbFilesContainerName> blobProvider,
            IEnumerable<Meta<Func<Uri, IPreviewProvider>>> providers)
        {
            _blobProvider = blobProvider;
            _providers = providers;
        }

        public IPreviewProvider PreviewFactory(string blobName)
        {
            if (!Uri.TryCreate(blobName, UriKind.Absolute, out Uri uri))
            {
                uri = _blobProvider.GetBlobUrl(blobName);
            }

            var process = _providers.FirstOrDefault(f =>
            {
                if (f.Metadata["AppenderName"] is string[] p)
                {
                    return p.Contains(Path.GetExtension(uri.AbsoluteUri), StringComparer.OrdinalIgnoreCase);
                }
                return false;
            });
            return process?.Value(uri);
            //return process.Value;
        }
    }
}
