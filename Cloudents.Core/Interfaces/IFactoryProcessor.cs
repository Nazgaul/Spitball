using Cloudents.Core.Storage;

namespace Cloudents.Core.Interfaces
{
    public interface IFactoryProcessor
    {
        IPreviewProvider PreviewFactory(string blobName);
    }
}
