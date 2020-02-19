namespace Cloudents.Infrastructure.Framework
{
    public interface IFactoryProcessor
    {
        IPreviewProvider PreviewFactory(string blobName);
    }
}
