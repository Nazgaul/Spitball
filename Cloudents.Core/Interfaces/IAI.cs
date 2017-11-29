﻿using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface IAI
    {
        Task<AiDto> InterpretStringAsync(string sentence);
    }

    public interface IDecision
    {
        VerticalEngineDto MakeDecision(AiDto aiResult);
    }

    public interface IEngineProcess
    {
        Task<VerticalEngineDto> ProcessRequestAsync(string str);
    }
}
