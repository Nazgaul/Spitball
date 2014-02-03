using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.File
{
    public interface IFileProcessorFactory
    {
        IContentProcessor GetProcessor(Uri contentUrl);
    }
}
