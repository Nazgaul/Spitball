using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        //private readonly IEnumerable<IContentProcessor> m_Processors;

        private readonly ILifetimeScope m_Container;
        public FileProcessorFactory(ILifetimeScope container)
        {
            m_Container = container;
        }

        public IContentProcessor GetProcessor<T, TU>(Uri contentUrl)
            where T : IPreviewContainer
            where TU : ICacheContainer
        {
            var blob = m_Container.Resolve<IBlobProvider2<T>>();
            var blob2 = m_Container.Resolve<IBlobProvider2<TU>>();
            var processors = m_Container.Resolve<IEnumerable<IContentProcessor>>(
                new NamedParameter("blobProviderPreview", blob),
                new NamedParameter("blobProviderCache", blob2));
            return processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
        }

        public IContentProcessor GetProcessor<T>(Uri contentUrl) where T : IPreviewContainer
        {
            var blob = m_Container.Resolve<IBlobProvider2<T>>();
            var blob2 = m_Container.Resolve<IBlobProvider2<CacheContainerName>>();
            var processors = m_Container.Resolve<IEnumerable<IContentProcessor>>(
                new NamedParameter("blobProviderPreview", blob), new NamedParameter("blobProviderCache", blob2));
            return processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
        }

        public IContentProcessor GetProcessor(Uri contentUrl)
        {
            var blob = m_Container.Resolve<IBlobProvider2<PreviewContainerName>>();
            if (blob == null)
            {
                TraceLog.WriteError("GetProcessor blob is null");
            }
            var blob2 = m_Container.Resolve<IBlobProvider2<CacheContainerName>>();
            if (blob2 == null)
            {
                TraceLog.WriteError("GetProcessor blob is null");
            }
            var processors = m_Container.Resolve<IEnumerable<IContentProcessor>>(
                new NamedParameter("blobProviderPreview", blob), new NamedParameter("blobProviderCache", blob2));
            return processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
        }
    }
}
