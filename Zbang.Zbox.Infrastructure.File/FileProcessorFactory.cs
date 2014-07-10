using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.File
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        private readonly IEnumerable<IContentProcessor> m_Processors;
        public FileProcessorFactory()
        {
            m_Processors = IocFactory.Unity.ResolveAll<IContentProcessor>();
        }
        public IContentProcessor GetProcessor(Uri contentUrl)
        {
           
            var processor = m_Processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
            return processor;
        }
    }

}
