using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface IAI
    {
        Task<AIDto> InterpretStringAsync(string sentence);
    }

    public interface IDecision
    {
        (AIResult result, AIDto data) MakeDecision(AIDto aiResult);
    }
}
