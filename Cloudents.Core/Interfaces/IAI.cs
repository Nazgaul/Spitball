﻿using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Interfaces
{
    public interface IAI
    {
        Task<AIDto> InterpretStringAsync(string sentence);
    }

    public interface IDecision
    {
        (AiResult result, AIDto data) MakeDecision(AIDto aiResult);
    }
}
