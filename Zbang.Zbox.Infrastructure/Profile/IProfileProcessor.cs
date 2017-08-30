using System.IO;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Profile
{
    public interface IProfileProcessor
    {
        Task<Stream> ProcessFileAsync(Stream stream, int width, int height);
    }
}
