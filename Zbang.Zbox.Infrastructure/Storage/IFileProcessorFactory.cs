using System;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IFileProcessorFactory
    {
        //IContentProcessor GetProcessor(Uri contentUrl, Type T);

        IContentProcessor GetProcessor<T, TU>(Uri contentUrl)
            where T : IPreviewContainer
            where TU : ICacheContainer;


        IContentProcessor GetProcessor<T>(Uri contentUrl) where T : IPreviewContainer;

        IContentProcessor GetProcessor(Uri contentUrl);
    }
}
