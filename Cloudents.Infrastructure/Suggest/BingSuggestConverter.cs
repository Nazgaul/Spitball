using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Suggest
{
    [UsedImplicitly]
    public class BingSuggestConverter : ITypeConverter<BingSuggest.SuggestionsObject,IEnumerable<string>>
    {
        public IEnumerable<string> Convert(BingSuggest.SuggestionsObject source, IEnumerable<string> destination, ResolutionContext context)
        {
            return source.SuggestionGroups.FirstOrDefault()?.SearchSuggestions.Select(s => s.DisplayText);
        }
    }
}
