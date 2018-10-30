namespace Cloudents.Infrastructure.Framework
{
    public interface IFactoryProcessor
    {
        IPreviewProvider2 PreviewFactory(string blobName);
    }
}
