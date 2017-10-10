using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Request;
using Microsoft.Cognitive.LUIS;

namespace Cloudents.Infrastructure.AI
{
    // ReSharper disable once InconsistentNaming - AI is Shorthand
   
    public class LuisAI : IAI, IDisposable
    {
        private readonly LuisClient m_Client;

        private readonly HashSet<string> m_SearchVariables = new HashSet<string>(new[] {"documents", "flashcards"},
            StringComparer.InvariantCultureIgnoreCase);

        private readonly HashSet<string> m_SearchTerms = new HashSet<string>(new[] { "isbn", "subject" },
            StringComparer.InvariantCultureIgnoreCase);

        public LuisAI(LuisClient client)
        {
            m_Client = client;
        }

        [Cache(TimeConst.Day, "ai")]
        public async Task<AIDto> InterpretStringAsync(string sentence)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));
            var result = await m_Client.Predict(sentence).ConfigureAwait(false);
            var entities = result.GetAllEntities();

            result.TopScoringIntent.Name.TryToEnum(out AIIntent intent);
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

        public void Dispose()
        {
            m_Client?.Dispose();
        }
    }
}
