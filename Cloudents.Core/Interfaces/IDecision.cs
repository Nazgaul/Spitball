using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IDecision
    {
        VerticalEngineDto MakeDecision(AiDto aiResult);
    }
}