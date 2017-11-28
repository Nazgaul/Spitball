using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.AI
{
    public class EngineProcess : IEngineProcess
    {
        private readonly IAI _ai;
        private readonly IDecision _mDecision;

        public EngineProcess(IAI ai, IDecision mDecision)
        {
            _ai = ai;
            _mDecision = mDecision;
        }

        public async Task<VerticalEngineDto> ProcessRequestAsync(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            var aiResult = await _ai.InterpretStringAsync(str).ConfigureAwait(false);
            var result = _mDecision.MakeDecision(aiResult);

            return result;
        }
    }
}
