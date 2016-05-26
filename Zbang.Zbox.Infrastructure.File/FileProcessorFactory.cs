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
        public IContentProcessor GetProcessor<T>(Uri contentUrl) where T : StorageContainerName
        {
            var blob = m_Container.Resolve<IBlobProvider2<T>>();
            var m_Processors = m_Container.Resolve<IEnumerable<IContentProcessor>>(new NamedParameter("blobProviderPreview", blob));
            var processor = m_Processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
            return processor;
        }

        public IContentProcessor GetProcessor(Uri contentUrl)
        {
            var blob = m_Container.Resolve<IBlobProvider2<PreviewContainerName>>();
            var m_Processors = m_Container.Resolve<IEnumerable<IContentProcessor>>(new NamedParameter("blobProviderPreview", blob));
            var processor = m_Processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
            return processor;
        }
    }

}
