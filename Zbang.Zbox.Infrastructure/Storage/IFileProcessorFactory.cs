﻿using System;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IFileProcessorFactory
    {
        IContentProcessor GetProcessor<T>(Uri contentUrl) where T : IPreviewContainer;

        IContentProcessor GetProcessor(Uri contentUrl);
    }
}
