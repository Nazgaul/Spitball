using System;

namespace Cloudents.Core.Interfaces
{
    public interface IReplaceImageProvider
    {
        Uri ChangeImageIfNeeded(string host, Uri image);
    }
}