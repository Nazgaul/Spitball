﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"),ApiController]
    public class SuggestController : ControllerBase
    {
        private readonly IIndex<Vertical, Lazy<ISuggestions>> _suggestions;
        private readonly Lazy<ISuggestions> _defaultSuggestions;

        public SuggestController(IIndex<Vertical, Lazy<ISuggestions>> suggestions, Lazy<ISuggestions> defaultSuggestions)
        {
            _suggestions = suggestions;
            _defaultSuggestions = defaultSuggestions;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync(string sentence, Vertical? vertical, CancellationToken token)
        {
            var suggestProvider = _defaultSuggestions;
            if (_suggestions.TryGetValue(vertical.GetValueOrDefault(Vertical.None), out var suggest))
            {
                suggestProvider = suggest;
            }
            return await suggestProvider.Value.SuggestAsync(sentence, token);
        }
    }
}