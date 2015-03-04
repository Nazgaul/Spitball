using System;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IFileProcessorFactory
    {
        IContentProcessor GetProcessor(Uri contentUrl);
    }
}
