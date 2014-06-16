using System;

namespace Zbang.Zbox.Infrastructure.File
{
    public interface IFileProcessorFactory
    {
        IContentProcessor GetProcessor(Uri contentUrl);
    }
}
