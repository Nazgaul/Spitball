using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IAI
    {
        Task<AiDto> InterpretStringAsync(string sentence, CancellationToken token);
    }

    public interface IEngineProcess
    {
        Task<VerticalEngineDto> ProcessRequestAsync(string str,CancellationToken token);
    }
}
