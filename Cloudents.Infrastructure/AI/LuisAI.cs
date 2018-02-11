using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.Cognitive.LUIS;

namespace Cloudents.Infrastructure.AI
{
    // ReSharper disable once InconsistentNaming - AI is Shorthand
    public class LuisAI : IAi, IDisposable
    {
        private readonly ILuisClient _client;

        private readonly HashSet<string> _searchVariables = new HashSet<string>(new[] { "documents", "flashcards" },
            StringComparer.InvariantCultureIgnoreCase);

        public LuisAI(ILuisClient client)
        {
            _client = client;
        }

        [Cache(TimeConst.Day, "ai")]
        public async Task<AiDto> InterpretStringAsync(string sentence, CancellationToken token)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));

            if (sentence.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length <= 1)
            {
                return new AiDto(AiIntent.Search, null, null, new[] { sentence }, null, null, null);
            }

            var result = await _client.Predict(sentence).ConfigureAwait(false);
            var entities = result.GetAllEntities();

            result.TopScoringIntent.Name.TryToEnum(out AiIntent intent);
            KeyValuePair<string, string>? searchType = null;
            string location = null, course = null, isbn = null;
            var terms = new List<string>();
            if (entities.Count == 0)
            {
                return new AiDto(intent, null, null, new[] { sentence }, null, null, null);
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
                if (entity.Name.Equals("subject", StringComparison.InvariantCultureIgnoreCase))
                {
                    terms.Add(entity.Value);
                }
                if (entity.Name.Equals("class", StringComparison.InvariantCultureIgnoreCase))
                {
                    course = entity.Value;
                }
                if (entity.Name.Equals("isbn", StringComparison.InvariantCultureIgnoreCase))
                {
                    isbn = entity.Value;
                }
                if (entity.Name.Equals("Weather.Location", StringComparison.InvariantCultureIgnoreCase))
                {
                    location = entity.Value;
                }
            }
            return new AiDto(intent, searchType, null, terms, location, course, isbn);
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
