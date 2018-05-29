using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Microsoft.Cognitive.LUIS;

namespace Cloudents.Infrastructure.AI
{
    // ReSharper disable once InconsistentNaming - AI is Shorthand
    public sealed class LuisAI : IAi, IDisposable
    {
        private readonly ILuisClient _client;

        private readonly HashSet<string> _searchVariables = new HashSet<string>(new[] { "documents", "flashcards" },
            StringComparer.InvariantCultureIgnoreCase);

        public LuisAI(ILuisClient client)
        {
            _client = client;
        }

        [Cache(TimeConst.Day, "ai",true)]
        public async Task<AiDto> InterpretStringAsync(string sentence, CancellationToken token)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));

            if (sentence.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length <= 1)
            {
                return new AiDto(AiIntent.Search, null, null, new[] { sentence }, null, null, null,sentence);
            }

            var result = await _client.Predict(sentence).ConfigureAwait(false);
            var entities = result.GetAllEntities();

            result.TopScoringIntent.Name.TryToEnum(out AiIntent intent);
            KeyValuePair<string, string>? searchType = null;
            string location = null, course = null, isbn = null;
            var terms = new List<string>();
            if (entities.Count == 0)
            {
                return new AiDto(intent, null, null, new[] { sentence }, null, null, null, sentence);
            }
            foreach (var entity in entities)
            {
                //if (entity.Name.Equals("university", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    university = entity.Value;
                //    continue;
                //}
                if (_searchVariables.Contains(entity.Name))
                {
                    searchType = new KeyValuePair<string, string>(entity.Name, entity.Value);
                    continue;
                }
                if (entity.Name.Equals("subject", StringComparison.OrdinalIgnoreCase))
                {
                    terms.Add(entity.Value);
                }
                if (entity.Name.Equals("class", StringComparison.OrdinalIgnoreCase))
                {
                    course = entity.Value;
                }
                if (entity.Name.Equals("isbn", StringComparison.OrdinalIgnoreCase))
                {
                    isbn = entity.Value;
                }
                if (entity.Name.Equals("Weather.Location", StringComparison.OrdinalIgnoreCase))
                {
                    location = entity.Value;
                }
            }
            return new AiDto(intent, searchType, null, terms, location, course, isbn, sentence);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
