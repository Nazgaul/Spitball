﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.AI
{
    public class EngineProcess : IEngineProcess
    {
        private readonly IAI _ai;
        private readonly IDecision _mDecision;
        private readonly IPlacesSearch _placesSearch;

        public EngineProcess(IAI ai, IDecision mDecision, IPlacesSearch placesSearch)
        {
            _ai = ai;
            _mDecision = mDecision;
            _placesSearch = placesSearch;
        }

        public async Task<VerticalEngineDto> ProcessRequestAsync(string str, CancellationToken token)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            var aiResult = await _ai.InterpretStringAsync(str, token).ConfigureAwait(false);
            var result = _mDecision.MakeDecision(aiResult);

            if (result is IVerticalLocation location && location.Location != null)
            {
                location.Cords = await _placesSearch.GeoCodingAsync(location.Location, token).ConfigureAwait(false);
            }

            return result;
        }
    }
}
