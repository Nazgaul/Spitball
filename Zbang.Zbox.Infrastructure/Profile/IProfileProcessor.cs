using System.IO;

namespace Zbang.Zbox.Infrastructure.Profile
{
    public interface IProfileProcessor
    {
        Stream ProcessFile(Stream stream, int width, int height);
    }
}
