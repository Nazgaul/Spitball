using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Microsoft.Cognitive.LUIS;

namespace Cloudents.Infrastructure.AI
{
    // ReSharper disable once InconsistentNaming - AI is Shorthand
    public class LuisAI : IAI
    {
        private readonly LuisClient m_Client = new LuisClient("a1a0245f-4cb3-42d6-8bb2-62b6cfe7d5a3", "6effb3962e284a9ba73dfb57fa1cfe40");

        private readonly HashSet<string> m_SearchVariables = new HashSet<string>(new[] {"documents", "flashcards"},
            StringComparer.InvariantCultureIgnoreCase);

        private readonly HashSet<string> m_SearchTerms = new HashSet<string>(new[] { "isbn", "subject" },
            StringComparer.InvariantCultureIgnoreCase);

        public async Task<AIDto> InterpretStringAsync(string sentence)
        {
            var result = await m_Client.Predict(sentence).ConfigureAwait(false);
            var entities = result.GetAllEntities();
            Enum.TryParse(result.TopScoringIntent.Name, out AIIntent intent);

            KeyValuePair<string, string>? searchType = null;
            string course = null;
            var terms = new List<string>();
            foreach (var entity in entities)
            {
                if (entity.Name.Equals("Class", StringComparison.InvariantCultureIgnoreCase))
                {
                    course = entity.Value;
                    continue;
                }
                if (m_SearchVariables.Contains(entity.Name))
                {
                    searchType = new KeyValuePair<string, string>(entity.Name, entity.Value);
                    continue;
                }
                if (m_SearchTerms.Contains(entity.Name))
                {
                    terms.Add(entity.Value);
                }
            }

            return new AIDto(intent, searchType, course, terms);
        }
    }
}
