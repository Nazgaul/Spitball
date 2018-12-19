using System;

namespace Cloudents.Application.Interfaces
{
    public interface IReplaceImageProvider
    {
        Uri ChangeImageIfNeeded(string host, Uri image);
    }
}