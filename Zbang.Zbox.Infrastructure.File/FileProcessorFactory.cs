using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        private readonly IEnumerable<IContentProcessor> m_Processors;
        public FileProcessorFactory(ILifetimeScope container)
        {
            m_Processors = container.Resolve<IEnumerable<IContentProcessor>>();
        }
        public IContentProcessor GetProcessor(Uri contentUrl)
        {

            var processor = m_Processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
            return processor;
        }
    }

}
