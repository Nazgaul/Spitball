using System.IO;

namespace Cloudents.Core.Interfaces
{
    public interface IImageProcessor
    {
        Stream ConvertToJpg(Stream read,  int quality = 80);
    }
}