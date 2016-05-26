using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        //private readonly IEnumerable<IContentProcessor> m_Processors;

        private readonly ILifetimeScope m_Container;
        public FileProcessorFactory(ILifetimeScope container)
        {
            m_Container = container;
            //container.Resolve<IEnumerable<IContentProcessor>>(new NamedParameter("x"),)

            //m_Processors = container.Resolve<IEnumerable<IContentProcessor>>();
        }
        public IContentProcessor GetProcessor<T, U>(Uri contentUrl)
            where T : IPreviewContainer
            where U : ICacheContainer
        {
            var blob = m_Container.Resolve<IBlobProvider2<T>>();
            var blob2 = m_Container.Resolve<IBlobProvider2<U>>();
            var processors = m_Container.Resolve<IEnumerable<IContentProcessor>>(
                new NamedParameter("blobProviderPreview", blob), new NamedParameter("blobProviderCache", blob2));
            var processor = processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
            return processor;
        }

        public IContentProcessor GetProcessor<T>(Uri contentUrl) where T : IPreviewContainer
        {
            var blob = m_Container.Resolve<IBlobProvider2<T>>();
            var blob2 = m_Container.Resolve<IBlobProvider2<CacheContainerName>>();
            var processors = m_Container.Resolve<IEnumerable<IContentProcessor>>(
                new NamedParameter("blobProviderPreview", blob), new NamedParameter("blobProviderCache", blob2));
            var processor = processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
            return processor;
        }

        public IContentProcessor GetProcessor(Uri contentUrl)
        {
            var blob = m_Container.Resolve<IBlobProvider2<PreviewContainerName>>();
            var blob2 = m_Container.Resolve<IBlobProvider2<CacheContainerName>>();
            var processors = m_Container.Resolve<IEnumerable<IContentProcessor>>(
                new NamedParameter("blobProviderPreview", blob), new NamedParameter("blobProviderCache", blob2));
            var processor = processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
            return processor;
        }
    }

}
